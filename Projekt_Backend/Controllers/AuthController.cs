using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_Backend.DTOs.AuthDTOs;
using Projekt_Backend.Models;
using Projekt_Backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Projekt_Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AcsolasContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthController(
            AcsolasContext db,
            IPasswordHasher hasher,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _db = db;
            _hasher = hasher;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        // Új felhasználó regisztrációja, email megerősítő link küldésével.
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register(
            [FromBody] RegisterRequestDto dto,
            CancellationToken ct)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            bool exists = await _db.Clients.AnyAsync(c => c.Email.ToLower() == email, ct);
            if (exists)
                return Conflict(new { message = "Ezzel az email címmel már létezik felhasználó." });

            var (hash, salt) = _hasher.HashPassword(dto.Password);
            var verificationToken = Guid.NewGuid().ToString("N");

            var client = new Client
            {
                Name = dto.Name.Trim(),
                Email = email,
                Telephone = dto.Telephone?.Trim() ?? string.Empty,
                BillingAddress = dto.BillingAddress?.Trim() ?? string.Empty,

                PasswordHash = hash,
                PasswordSalt = salt,
                PasswordIterations = _hasher.Iterations,

                Role = "User",
                IsActive = true,

                EmailConfirmed = false,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            _db.Clients.Add(client);
            await _db.SaveChangesAsync(ct);

            var verifyUrl = $"http://localhost:5173/verify-email?token={verificationToken}";

            try
            {
                await _emailService.SendEmailAsync(
                    client.Email,
                    "Email cím megerősítése",
                    $@"Köszönjük a regisztrációt!

Az email címed megerősítéséhez kattints az alábbi linkre:

{verifyUrl}

A link 24 óráig érvényes."
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "A regisztráció sikerült, de az email küldés nem sikerült.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }

            return Created("", new RegisterResponseDto
            {
                ClientId = client.ClientId,
                Email = client.Email
            });
        }

        // Bejelentkezés csak aktív és emailben megerősített felhasználónak.
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(
            [FromBody] LoginRequestDto dto,
            CancellationToken ct)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var client = await _db.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email, ct);

            if (client is null)
                return Unauthorized(new { message = "Hibás email vagy jelszó." });

            if (!client.IsActive)
                return StatusCode(403, new { message = "A felhasználó le van tiltva." });

            if (!client.EmailConfirmed)
                return StatusCode(403, new { message = "Az email cím még nincs megerősítve." });

            bool ok = _hasher.VerifyPassword(
                dto.Password,
                client.PasswordHash,
                client.PasswordSalt,
                client.PasswordIterations);

            if (!ok)
                return Unauthorized(new { message = "Hibás email vagy jelszó." });

            var token = _tokenService.CreateToken(
                client.ClientId,
                client.Email,
                client.Name,
                client.Role
            );

            return Ok(new LoginResponseDto
            {
                ClientId = client.ClientId,
                Email = client.Email,
                Name = client.Name,
                Role = client.Role,
                Token = token
            });
        }

        // Email megerősítő link kezelése.
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Hiányzó token." });

            var client = await _db.Clients
                .FirstOrDefaultAsync(c => c.EmailVerificationToken == token, ct);

            if (client is null)
                return BadRequest(new { message = "Érvénytelen token." });

            if (client.EmailConfirmed)
                return Ok(new { message = "Az email cím már korábban meg lett erősítve." });

            if (client.EmailVerificationTokenExpiresAt is null ||
                client.EmailVerificationTokenExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new { message = "A token lejárt." });
            }

            client.EmailConfirmed = true;
            client.EmailVerificationToken = null;
            client.EmailVerificationTokenExpiresAt = null;

            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "Az email cím megerősítve." });
        }

        // JWT token visszavonása kijelentkezéskor.
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var jti =
                User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ??
                User.FindFirst("jti")?.Value;

            var sub =
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                User.FindFirst("sub")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(jti) || string.IsNullOrWhiteSpace(sub))
                return BadRequest(new { message = "Missing jti/sub claim." });

            if (!int.TryParse(sub, out var clientId))
                return BadRequest(new { message = "Invalid sub claim (not int)." });

            var expStr =
                User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value ??
                User.FindFirst("exp")?.Value;

            DateTime expiresAt = DateTime.UtcNow.AddMinutes(30);

            if (long.TryParse(expStr, out var exp))
                expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;

            bool exists = await _db.RevokedTokens.AnyAsync(x => x.Jti == jti, ct);

            if (!exists)
            {
                _db.RevokedTokens.Add(new RevokedToken
                {
                    Jti = jti,
                    ClientId = clientId,
                    RevokedAt = DateTime.UtcNow,
                    ExpiresAt = expiresAt
                });

                await _db.SaveChangesAsync(ct);
            }

            return NoContent();
        }
    }
}


