using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt_Backend.DTOs.CartDTOs;
using Projekt_Backend.DTOs.OrdersDTOs;
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

        public CartController(IOrderService orders)
        {
            _orders = orders;
        }

        // Frontend kosár -> rendelés (csak bejelentkezett user)
        [Authorize]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDTO dto)
        {
            // 1) ClientId a tokenből (sub)
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
                return Unauthorized(new { message = "Érvénytelen token (nincs sub/clientId)." });

            // 2) Alap validáció (ne engedjünk üres kosarat)
            if (dto == null)
                return BadRequest(new { message = "Hiányzó kérés törzs." });

            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(new { message = "A kosár üres." });

            // 3) Egyszerű védelmek
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

            // opcionális: duplikált termékek összevonása (frontend hibák ellen)
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

            // 4) OrderCreateDTO építés (clientId már a tokenből)
            var orderDto = new OrderCreateDTO
            {
                ClientId = clientId,
                Comment = dto.Comment?.Trim() ?? string.Empty,
                Items = merged
            };

            try
            {
                var created = await _orders.CreateAsync(orderDto);
                return Ok(created);
            }
            catch (InvalidOperationException ex)
            {
                // pl. üres kosár / nem létező termék / nem létező ügyfél
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}