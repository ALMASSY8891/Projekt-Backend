using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_Backend.Dtos.AuthAuthenticatorDTOs;
using Projekt_Backend.Models;

namespace Projekt_Backend.Controllers
{
    [ApiController]
    [Route("api/register")]
    public class RegisterController : ControllerBase
    {
        private readonly AcsolasContext _db;

        public RegisterController(AcsolasContext db)
        {
            _db = db;
        }

        [HttpPost("Create_Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto req)
        {
            // Email 
            var email = req.Email.Trim().ToLowerInvariant();

            // hiba, ha már létezik
            var exists = await _db.Clients.AnyAsync(c => c.Email.ToLower() == email);
            if (exists)
                return Conflict(new { message = "Ezzel az email címmel már létezik fiók." });
            // Új kliens létrehozása
            var client = new Client
            {
                Name = req.Name.Trim(),
                Email = email,
                Password = req.Password, 
                Telephone = req.Telephone.Trim(),
                BillingAddress = req.BillingAddress.Trim()
            };

            _db.Clients.Add(client);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Race condition esetén a UNIQUE fogja meg
                return Conflict(new { message = "Ezzel az email címmel már létezik fiók." });
            }

            return Ok(new { message = "Regisztráció sikeres.", clientId = client.ClientId });
        }
    }
}
