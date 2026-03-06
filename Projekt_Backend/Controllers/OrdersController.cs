using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_Backend.DTOs.OrdersDTOs;
using Projekt_Backend.Models;
using Projekt_Backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Projekt_Backend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly AcsolasContext _db;

        public OrdersController(
            IOrderService service,
            IEmailService emailService,
            IConfiguration config,
            AcsolasContext db)
        {
            _service = service;
            _emailService = emailService;
            _config = config;
            _db = db;
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
                return Unauthorized(new { message = "Érvénytelen token (nincs sub/clientId)." });

            return Ok(await _service.GetMyAsync(clientId));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            if (User.IsInRole("Admin"))
                return Ok(order);

            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
                return Unauthorized(new { message = "Érvénytelen token (nincs sub/clientId)." });

            if (order.ClientId != clientId)
                return Forbid();

            return Ok(order);
        }

        // Csak swagger tesztelésre vagy manuális rendelésfelvételre használható.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDTO dto)
        {
            var created = await _service.CreateAsync(dto);
            var adminEmail = _config["AdminEmail"] ?? "tesztacsolas@gmail.com";
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientId == created.ClientId);

            try
            {
                if (client != null)
                {
                    await _emailService.SendEmailAsync(
                        client.Email,
                        "Rendelés visszaigazolás",
                        $"Kedves {client.Name}!\n\n" +
                        $"A rendelésed rögzítve lett.\n\n" +
                        $"Rendelés azonosító: {created.OrderId}\n" +
                        $"Rendelés dátuma: {created.OrderDate:yyyy.MM.dd HH:mm}\n" +
                        $"Státusz: {created.OrderStatus}\n" +
                        $"Végösszeg: {created.TotalGross} Ft\n\n" +
                        $"Köszönjük a rendelést!"
                    );
                }

                await _emailService.SendEmailAsync(
                adminEmail,
                "[ADMIN] Új rendelés",
                $"Új rendelés jött létre.\n\n" +
                $"Rendelés azonosító: {created.OrderId}\n" +
                $"Ügyfél: {(client != null ? $"{client.Name} ({client.Email})" : $"ismeretlen ügyfél (ClientId: {created.ClientId})")}\n" +
                $"Rendelés dátuma: {created.OrderDate:yyyy.MM.dd HH:mm}\n" +
                $"Státusz: {created.OrderStatus}\n" +
                $"Végösszeg: {created.TotalGross} Ft"
            );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "A rendelés létrejött, de az email küldés nem sikerült.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }

            return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var ok = await _service.DeleteAsync(id);
            if (!ok)
                return NotFound();

            var adminEmail = _config["AdminEmail"] ?? "tesztacsolas@gmail.com";
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientId == order.ClientId);

            try
            {
                if (client != null)
                {
                    await _emailService.SendEmailAsync(
                        client.Email,
                        "Rendelés törölve",
                        $"Kedves {client.Name}!\n\n" +
                        $"A rendelésed törlésre került.\n\n" +
                        $"Rendelés azonosító: {order.OrderId}\n" +
                        $"Korábbi státusz: {order.OrderStatus}"
                    );
                }

                await _emailService.SendEmailAsync(
                adminEmail,
                "[ADMIN] Rendelés törölve",
                $"Egy rendelés törölve lett.\n\n" +
                $"Rendelés azonosító: {order.OrderId}\n" +
                $"Ügyfél: {(client != null ? $"{client.Name} ({client.Email})" : $"ismeretlen ügyfél (ClientId: {order.ClientId})")}\n" +
                $"Korábbi státusz: {order.OrderStatus}"
            );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "A rendelés törölve lett, de az email küldés nem sikerült.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatusUpdateDTO dto)
        {
            var orderBefore = await _service.GetByIdAsync(id);
            if (orderBefore == null)
                return BadRequest(new { message = "Érvénytelen rendelés vagy státusz." });

            var oldStatus = orderBefore.OrderStatus;

            var ok = await _service.UpdateStatusAsync(id, dto.NewStatus);
            if (!ok)
                return BadRequest(new { message = "Érvénytelen rendelés vagy státusz." });

            var orderAfter = await _service.GetByIdAsync(id);
            if (orderAfter == null)
                return NoContent();

            if (oldStatus != orderAfter.OrderStatus)
            {
                var adminEmail = _config["AdminEmail"] ?? "tesztacsolas@gmail.com";
                var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientId == orderAfter.ClientId);

                try
                {
                    if (client != null)
                    {
                        await _emailService.SendEmailAsync(
                            client.Email,
                            "Rendelés állapota módosult",
                            $"Kedves {client.Name}!\n\n" +
                            $"A rendelésed állapota megváltozott.\n\n" +
                            $"Rendelés azonosító: {orderAfter.OrderId}\n" +
                            $"Régi státusz: {oldStatus}\n" +
                            $"Új státusz: {orderAfter.OrderStatus}\n" +
                            $"Végösszeg: {orderAfter.TotalGross} Ft"
                        );
                    }

                    await _emailService.SendEmailAsync(
                      adminEmail,
                      "[ADMIN] Rendelés státusz módosítva",
                      $"A rendelés státusza módosult.\n\n" +
                      $"Rendelés azonosító: {orderAfter.OrderId}\n" +
                      $"Ügyfél: {(client != null ? $"{client.Name} ({client.Email})" : $"ismeretlen ügyfél (ClientId: {orderAfter.ClientId})")}\n" +
                      $"Régi státusz: {oldStatus}\n" +
                      $"Új státusz: {orderAfter.OrderStatus}\n" +
                      $"Végösszeg: {orderAfter.TotalGross} Ft"
                  );
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        message = "A státusz módosult, de az email küldés nem sikerült.",
                        error = ex.Message,
                        innerError = ex.InnerException?.Message
                    });
                }
            }

            return NoContent();
        }
    }
}