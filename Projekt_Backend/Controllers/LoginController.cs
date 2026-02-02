using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_Backend.Dtos;
using Projekt_Backend.Dtos.AuthenticatorDTOs;
using Projekt_Backend.Models;

namespace Projekt_Backend.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly AcsolasContext _db;

        public LoginController(AcsolasContext db)
        {
            _db = db;
        }
        // POST /api/login
        [HttpPost("Create_Login_uId")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto req)
        {
            //kisbetűsre alakítja az emailt
            var email = req.Email.Trim().ToLowerInvariant();
            //megkeresi a client táblában
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.Email.ToLower() == email);
            //összehasonlítja a session táblában használt jelszóval
            if (client is null || client.Password != req.Password)
                return Unauthorized(new { message = "Hibás email vagy jelszó." });
            //létrehoz egy új tokent
            var token = Guid.NewGuid().ToString("N");
            //elmenti az AuthoritySessions táblába
            _db.AuthoritySessions.Add(new AuthoritySession
            {
                ClientId = client.ClientId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            });

            await _db.SaveChangesAsync();
            //visszaadja a választ
            return Ok(new AuthResponseDto
            {
                ClientId = client.ClientId,
                Email = client.Email,
                Token = token
            });
        }

        // token az URL-ben
        // POST /api/login/logout/{uId}
        [HttpPost("logout/{uId}")]
        public async Task<IActionResult> Logout(string uId)
        {
            /*
             * DEMO / ISKOLAI PROJEKT MEGJEGYZÉS:
             * A DEMO_ADMIN_TOKEN egy előre rögzített admin session,
             * amit bemutatókhoz és teszteléshez használunk.
             * Ezt a tokent nem engedjük kijelentkeztetni,
             * hogy ne kelljen minden bemutató előtt újra létrehozni.
             *
             * Valós (éles) rendszerben természetesen MINDEN token
             * logoutolható lenne.
             */
            if (uId == "DEMO_ADMIN_TOKEN")
                return Ok(new { message = "DEMO admin token – kijelentkezés tiltva." });

            // Normál felhasználói tokenek kezelése
            var session = await _db.AuthoritySessions
                .FirstOrDefaultAsync(s => s.Token == uId && !s.IsRevoked);

            // Ha nincs ilyen aktív session, nincs mit visszavonni
            if (session is null)
                return Ok(new { message = "OK" });

            // Token érvénytelenítése (kijelentkezés)
            session.IsRevoked = true;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Kijelentkezve." });
        }

    }
}
