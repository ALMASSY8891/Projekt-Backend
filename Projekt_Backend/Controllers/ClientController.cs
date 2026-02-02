using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_Backend.Dtos.Client;
using Projekt_Backend.Models;
using Projekt_Backend.Security;

namespace Projekt_Backend.Controllers
{
    [ApiController]
    [Route("api/client")]
    public class ClientController : ControllerBase
    {
        // olvasható adatokat szolgáltató végpontok (GET) és módosító végpontok (PUT) a kliens saját adataihoz
        private readonly AcsolasContext _db;
        private readonly TokenGuard _guard;

        // Konstruktor
        public ClientController(AcsolasContext db, TokenGuard guard)
        {
            _db = db;
            _guard = guard;
        }
        // GET: /api/client/me/{uId}
        [HttpGet("Read_Client/{uId}")]
        public async Task<IActionResult> GetMe(string uId)
        {
            // Token alapján lekéri a kliens adatait
            var clientId = await _guard.GetClientIdAsync(uId);
            if (clientId is null) return Unauthorized(new { message = "Érvénytelen token." });
            // Lekéri a kliens adatait az adatbázisból
            var client = await _db.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == clientId.Value);
            if (client is null) return NotFound();
            // Visszaadja a kliens adatait
            return Ok(new
            {
                client.ClientId,
                client.Name,
                client.Email,
                client.Telephone,
                client.BillingAddress
            });
        }

        [HttpPut("Update_Token/{uId}")]
        public async Task<IActionResult> UpdateMe(string uId, [FromBody] ClientUpdateDto dto)
        {
            // Token alapján lekéri a kliens adatait
            var clientId = await _guard.GetClientIdAsync(uId);
            if (clientId is null) return Unauthorized(new { message = "Érvénytelen token." });
            // Lekéri a kliens adatait az adatbázisból
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId.Value);
            if (client is null) return NotFound();

            // Belépési adatok (Email/Password) itt NEM módosíthatók.
            client.Name = dto.Name.Trim();
            client.Telephone = dto.Telephone.Trim();
            client.BillingAddress = dto.BillingAddress.Trim();

            await _db.SaveChangesAsync();
            return Ok(new { message = "Mentve." });
        }
    }
}

