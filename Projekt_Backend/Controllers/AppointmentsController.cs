using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_Backend.DTOs.AppointmentsDTOs;
using Projekt_Backend.Models;
using Projekt_Backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Projekt_Backend.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;
        private readonly IEmailService _emailService;
        private readonly AcsolasContext _db;
        private readonly IConfiguration _config;

        public AppointmentsController(
            IAppointmentService service,
            IEmailService emailService,
            AcsolasContext db,
            IConfiguration config)
        {
            _service = service;
            _emailService = emailService;
            _db = db;
            _config = config;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [Authorize]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
                return Unauthorized(new { message = "Érvénytelen token." });

            return Ok(await _service.GetMyAsync(clientId));
        }

        [Authorize]
        [HttpPost("{id}/cancel-mine")]
        public async Task<IActionResult> CancelMine(int id)
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
                return Unauthorized(new { message = "Érvénytelen token." });

            var appointment = await _db.Appointments
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.ClientId == clientId);

            var ok = await _service.CancelMineAsync(id, clientId);

            if (!ok)
                return BadRequest(new { message = "Nem lemondható." });

            if (appointment != null)
            {
                var adminEmail = _config["AdminEmail"] ?? "tesztacsolas@gmail.com";

                try
                {
                    await _emailService.SendEmailAsync(
                        appointment.Client.Email,
                        "Időpontfoglalás lemondva",
                        $"A foglalásod lemondásra került.\n\n" +
                        $"Kezdés: {appointment.StartTime:yyyy.MM.dd HH:mm}\n" +
                        $"Vége: {appointment.EndTime:yyyy.MM.dd HH:mm}"
                    );

                    await _emailService.SendEmailAsync(
                        adminEmail,
                        "[ADMIN] Időpont lemondva",
                        $"A felhasználó lemondta az időpontját.\n\n" +
                        $"Időpont azonosító: {appointment.AppointmentId}\n" +
                        $"Ügyfél: {appointment.Client.Email}\n" +
                        $"Kezdés: {appointment.StartTime:yyyy.MM.dd HH:mm}\n" +
                        $"Vége: {appointment.EndTime:yyyy.MM.dd HH:mm}"
                    );
                }
                catch
                {
                }
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateDTO dto)
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
                return Unauthorized(new { message = "Érvénytelen token." });

            if (dto.EndTime <= dto.StartTime)
                return BadRequest(new { message = "Érvénytelen időtartam." });

            var created = await _service.CreateAsync(clientId, dto);

            if (created == null)
                return Conflict(new { message = "Az időpont már foglalt." });

            var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
            var adminEmail = _config["AdminEmail"] ?? "tesztacsolas@gmail.com";

            if (client != null)
            {
                try
                {
                    await _emailService.SendEmailAsync(
                        client.Email,
                        "Időpontfoglalás rögzítve",
                        $"Az időpontfoglalásod rögzítve lett.\n\n" +
                        $"Kezdés: {created.StartTime:yyyy.MM.dd HH:mm}\n" +
                        $"Vége: {created.EndTime:yyyy.MM.dd HH:mm}"
                    );

                    await _emailService.SendEmailAsync(
                        adminEmail,
                        "[ADMIN] Új időpontfoglalás",
                        $"Új időpontfoglalás érkezett.\n\n" +
                        $"Ügyfél: {client.Email}\n" +
                        $"Kezdés: {created.StartTime:yyyy.MM.dd HH:mm}\n" +
                        $"Vége: {created.EndTime:yyyy.MM.dd HH:mm}"
                    );
                }
                catch
                {
                }
            }

            return CreatedAtAction(nameof(GetMine), new { }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin")]
        public async Task<IActionResult> CreateAdmin(AppointmentAdminCreateDTO dto)
        {
            if (dto.EndTime <= dto.StartTime)
                return BadRequest(new { message = "Érvénytelen időtartam." });

            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var adminClientId))
                return Unauthorized(new { message = "Érvénytelen token." });

            var created = await _service.CreateAdminAsync(adminClientId, dto);

            if (created == null)
                return Conflict(new { message = "Az időpont már foglalt." });

            var adminEmail = _config["AdminEmail"] ?? "tesztacsolas@gmail.com";

            try
            {
                if (dto.ClientId.HasValue)
                {
                    var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientId == dto.ClientId.Value);

                    if (client != null)
                    {
                        await _emailService.SendEmailAsync(
                            client.Email,
                            "Új időpont került rögzítésre",
                            $"Az admin új időpontot rögzített számodra.\n\n" +
                            $"Kezdés: {created.StartTime:yyyy.MM.dd HH:mm}\n" +
                            $"Vége: {created.EndTime:yyyy.MM.dd HH:mm}"
                        );

                        await _emailService.SendEmailAsync(
                            adminEmail,
                            "[ADMIN] Admin időpontot hozott létre",
                            $"Az admin új időpontot hozott létre egy ügyfélnek.\n\n" +
                            $"Ügyfél: {client.Email}\n" +
                            $"Kezdés: {created.StartTime:yyyy.MM.dd HH:mm}\n" +
                            $"Vége: {created.EndTime:yyyy.MM.dd HH:mm}"
                        );
                    }
                }
                else
                {
                    await _emailService.SendEmailAsync(
                        adminEmail,
                        "[ADMIN] Admin blokk létrehozva",
                        $"Admin blokk került létrehozásra.\n\n" +
                        $"Kezdés: {created.StartTime:yyyy.MM.dd HH:mm}\n" +
                        $"Vége: {created.EndTime:yyyy.MM.dd HH:mm}"
                    );
                }
            }
            catch
            {
            }

            return Ok(created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var appointment = await _db.Appointments
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            var ok = await _service.ConfirmAsync(id);

            if (!ok)
                return BadRequest(new { message = "Érvénytelen foglalás vagy ütköző időpont." });

            if (appointment != null)
            {
                var adminEmail = _config["AdminEmail"] ?? "tesztacsolas@gmail.com";

                try
                {
                    await _emailService.SendEmailAsync(
                        appointment.Client.Email,
                        "Időpontfoglalás jóváhagyva",
                        $"Az időpontfoglalásod jóváhagyásra került.\n\n" +
                        $"Kezdés: {appointment.StartTime:yyyy.MM.dd HH:mm}\n" +
                        $"Vége: {appointment.EndTime:yyyy.MM.dd HH:mm}"
                    );

                    await _emailService.SendEmailAsync(
                        adminEmail,
                        "[ADMIN] Időpont jóváhagyva",
                        $"Az időpontfoglalás jóváhagyva.\n\n" +
                        $"Időpont azonosító: {appointment.AppointmentId}\n" +
                        $"Ügyfél: {appointment.Client.Email}\n" +
                        $"Kezdés: {appointment.StartTime:yyyy.MM.dd HH:mm}\n" +
                        $"Vége: {appointment.EndTime:yyyy.MM.dd HH:mm}"
                    );
                }
                catch
                {
                }
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _db.Appointments
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            var ok = await _service.CancelAsync(id);

            if (!ok)
                return BadRequest(new { message = "Érvénytelen foglalás." });

            if (appointment != null)
            {
                var adminEmail = _config["AdminEmail"] ?? "tesztacsolas@gmail.com";

                try
                {
                    await _emailService.SendEmailAsync(
                        appointment.Client.Email,
                        "Időpontfoglalás törölve",
                        $"Az időpontfoglalásod törlésre került.\n\n" +
                        $"Kezdés: {appointment.StartTime:yyyy.MM.dd HH:mm}\n" +
                        $"Vége: {appointment.EndTime:yyyy.MM.dd HH:mm}"
                    );

                    await _emailService.SendEmailAsync(
                        adminEmail,
                        "[ADMIN] Időpont törölve",
                        $"Az időpontfoglalás törölve lett.\n\n" +
                        $"Időpont azonosító: {appointment.AppointmentId}\n" +
                        $"Ügyfél: {appointment.Client.Email}\n" +
                        $"Kezdés: {appointment.StartTime:yyyy.MM.dd HH:mm}\n" +
                        $"Vége: {appointment.EndTime:yyyy.MM.dd HH:mm}"
                    );
                }
                catch
                {
                }
            }

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("busy")]
        public async Task<IActionResult> GetBusy([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (to <= from)
                return BadRequest(new { message = "Érvénytelen intervallum." });

            var busy = await _service.GetBusyAsync(from, to);
            return Ok(busy);
        }
    }
}