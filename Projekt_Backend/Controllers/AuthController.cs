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

        public AuthController(AcsolasContext db, IPasswordHasher hasher, ITokenService tokenService)
        {
            _db = db;
            _hasher = hasher;
            _tokenService = tokenService;
        }
        // Ez a művelet új felhasználó regisztrációját valósítja meg. A kliens által küldött adatok (név, email, jelszó stb.) alapján létrehoz egy új Client entitást, amelyet elment az adatbázisba. A jelszót biztonságosan hash-eli és salt-eli a IPasswordHasher szolgáltatás segítségével. Ha a regisztráció sikeres, akkor egy 201 Created választ ad vissza a létrehozott felhasználó adataival.
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto dto, CancellationToken ct)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            bool exists = await _db.Clients.AnyAsync(c => c.Email.ToLower() == email, ct);
            if (exists)
                return Conflict(new { message = "Ezzel az email címmel már létezik felhasználó." });

            var (hash, salt) = _hasher.HashPassword(dto.Password);

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

                
                IsActive = true
            };

            _db.Clients.Add(client);
            await _db.SaveChangesAsync(ct);

            return Created("", new RegisterResponseDto
            {
                ClientId = client.ClientId,
                Email = client.Email
            });
        }
        // Ez a művelet felhasználói bejelentkezést valósít meg. A kliens által küldött email és jelszó alapján ellenőrzi a felhasználó létezését és hitelesítését. Ha a hitelesítés sikeres, akkor létrehoz egy JWT tokent a ITokenService segítségével, amely tartalmazza a felhasználó azonosítóit és szerepkörét. A token visszaadásra kerül a kliensnek egy LoginResponseDto-ban, amely tartalmazza a felhasználó adatait és a token értékét.
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var client = await _db.Clients.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email, ct);

            if (client is null)
                return Unauthorized(new { message = "Hibás email vagy jelszó." });

            // ha bevezeted az IsActive-t, akkor tiltott user ne tudjon belépni
            if (!client.IsActive)
                return StatusCode(403, new { message = "A felhasználó le van tiltva." });

            bool ok = _hasher.VerifyPassword(dto.Password, client.PasswordHash, client.PasswordSalt, client.PasswordIterations);
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
        [Authorize]// Ez a művelet csak hitelesített felhasználók számára engedélyezett, ami azt jelenti, hogy a kliensnek érvényes JWT tokennel kell rendelkeznie a kérés elküldésekor. Ez a művelet a JWT token visszavonását (revocation) valósítja meg, ami azt jelenti, hogy a token érvénytelen lesz a továbbiakban, még akkor is, ha az eredetileg még nem járt le. Ez egy fontos biztonsági intézkedés, különösen akkor, ha egy felhasználó kijelentkezik vagy ha egy token kompromittálódik.
        [HttpPost("logout")]// Ez a művelet a JWT token visszavonását (revocation) valósítja meg, ami azt jelenti, hogy a token érvénytelen lesz a továbbiakban, még akkor is, ha az eredetileg még nem járt le. Ez egy fontos biztonsági intézkedés, különösen akkor, ha egy felhasználó kijelentkezik vagy ha egy token kompromittálódik.
        public async Task<IActionResult> Logout()
        {
            // jti: próbáld standard + raw néven is
            var jti =
                User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ??
                User.FindFirst("jti")?.Value;

            // sub: próbáld standard + raw + nameidentifier néven is
            var sub =
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                User.FindFirst("sub")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(jti) || string.IsNullOrWhiteSpace(sub))
                return BadRequest(new { message = "Missing jti/sub claim." });

            if (!int.TryParse(sub, out var clientId))
                return BadRequest(new { message = "Invalid sub claim (not int)." });

            // exp: standard + raw néven is
            var expStr =
                User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value ??
                User.FindFirst("exp")?.Value;

            DateTime expiresAt = DateTime.UtcNow.AddMinutes(30); // fallback
            if (long.TryParse(expStr, out var exp))
                expiresAt = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;

            var exists = await _db.RevokedTokens.AnyAsync(x => x.Jti == jti);
            if (!exists)
            {
                _db.RevokedTokens.Add(new RevokedToken
                {
                    Jti = jti,
                    ClientId = clientId,
                    RevokedAt = DateTime.UtcNow,
                    ExpiresAt = expiresAt
                });

                await _db.SaveChangesAsync();
            }

            return NoContent();
        }

    }
}



