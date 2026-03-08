using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_Backend.DTOs.CartDTOs;
using Projekt_Backend.DTOs.OrdersDTOs;
using Projekt_Backend.Models;
using Projekt_Backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Projekt_Backend.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly IOrderService _orders;
        private readonly IEmailService _emailService;
        private readonly AcsolasContext _db;
        private readonly IConfiguration _config;

        public CartController(
            IOrderService orders,
            IEmailService emailService,
            AcsolasContext db,
            IConfiguration config)
        {
            _orders = orders;
            _emailService = emailService;
            _db = db;
            _config = config;
        }

        [Authorize]
        [HttpPost("checkout")]// Kosár tartalmának véglegesítése és rendelés létrehozása.
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDTO dto)
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
                return Unauthorized(new { message = "Érvénytelen token (nincs sub/clientId)." });

            if (dto == null)
                return BadRequest(new { message = "Hiányzó kérés törzs." });

            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(new { message = "A kosár üres." });

            const int maxItems = 200;
            const int maxQuantity = 999;

            if (dto.Items.Count > maxItems)
                return BadRequest(new { message = $"Túl sok tétel a kosárban (max {maxItems})." });

            if (dto.Items.Any(i => i.ProductId <= 0))
                return BadRequest(new { message = "Érvénytelen ProductId." });

            if (dto.Items.Any(i => i.Quantity <= 0))
                return BadRequest(new { message = "A mennyiség nem lehet 0 vagy negatív." });

            if (dto.Items.Any(i => i.Quantity > maxQuantity))
                return BadRequest(new { message = $"A mennyiség túl nagy (max {maxQuantity})." });

            var merged = dto.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new OrderItemCreateDTO
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            if (merged.Any(i => i.Quantity > maxQuantity))
                return BadRequest(new { message = $"Összevonás után valamelyik mennyiség túl nagy (max {maxQuantity})." });

            var orderDto = new OrderCreateDTO
            {
                ClientId = clientId,
                Comment = dto.Comment?.Trim() ?? string.Empty,
                Items = merged
            };

            try
            {
                var created = await _orders.CreateAsync(orderDto);
                var adminEmail = _config["AdminEmail"] ?? "tesztacsolas@gmail.com";
                var client = await _db.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
                // Email küldése ügyfélnek és adminnak
                try
                {
                    if (client != null)
                    {
                        await _emailService.SendEmailAsync(
                            client.Email,
                            "Rendelés visszaigazolás",
                            $"Kedves {client.Name}!\n\n" +
                            $"A rendelésedet megkaptuk.\n\n" +
                            $"Rendelés azonosító: {created.OrderId}\n" +
                            $"Rendelés dátuma: {created.OrderDate:yyyy.MM.dd HH:mm}\n" +
                            $"Státusz: {created.OrderStatus}\n" +
                            $"Végösszeg: {created.TotalGross} Ft\n\n" +
                            $"Köszönjük a rendelést!"
                        );
                    }

                    await _emailService.SendEmailAsync(
                        adminEmail,
                        "[ADMIN] Új rendelés érkezett",
                        $"Új rendelés érkezett a webshopból.\n\n" +
                        $"Rendelés azonosító: {created.OrderId}\n" +
                        $"Ügyfél azonosító: {created.ClientId}\n" +
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

                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}