using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_Backend.Models;
using Projekt_Backend.Security;

namespace Projekt_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesztController : ControllerBase
    {
        private readonly AcsolasContext _db;
        private readonly TokenGuard _guard;

        public TesztController(AcsolasContext db, TokenGuard guard)
        {
            _db = db;
            _guard = guard;
        }

        // GET: /api/teszt/me/{uId}
        [HttpGet("me/{uId}")]
        public async Task<IActionResult> GetMe(string uId)
        {
            var clientId = await _guard.GetClientIdAsync(uId);
            if (clientId is null) return Unauthorized(new { message = "Érvénytelen token." });

            var client = await _db.Clients.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClientId == clientId.Value);

            if (client is null) return NotFound();

            return Ok(new { client.ClientId, client.Email, client.Name });
        }
    }
}

