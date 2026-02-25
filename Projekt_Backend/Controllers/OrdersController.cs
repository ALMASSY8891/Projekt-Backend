using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt_Backend.DTOs.OrdersDTOs;
using Projekt_Backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Projekt_Backend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;//DI:a controller csak az interfacet ismeri, nem a konkrét implementációt, így könnyen cserélhető a szolgáltatás implementációja anélkül, hogy a controller kódját módosítani kellene.
        //DI konstruktor
        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]//Ez a művelet lekéri az összes rendelést. Az async/await használata lehetővé teszi, hogy a művelet aszinkron módon fusson, így nem blokkolja a szerver erőforrásait, amíg az adatbázisból lekéri a rendeléseket.
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [Authorize]
        [HttpGet("mine")]//Ez a művelet lekéri a bejelentkezett felhasználó rendeléseit. A JWT tokenből kinyeri a sub claim értékét, ami a clientId-t tartalmazza, majd ezt használja a szolgáltatás GetMyAsync metódusának meghívásához. Ha a token érvénytelen vagy nincs sub claim, akkor Unauthorized választ ad vissza.
        public async Task<IActionResult> GetMine()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
                return Unauthorized(new { message = "Érvénytelen token (nincs sub/clientId)." });

            return Ok(await _service.GetMyAsync(clientId));
        }

        [Authorize]
        [HttpGet("{id}")]//Ez a művelet lekéri egy adott rendelést azonosító alapján. Az adminisztrátorok minden rendelést láthatnak, míg a normál felhasználók csak a saját rendeléseiket. Ha a rendelés nem található, akkor NotFound választ adunk vissza, különben pedig az Ok választ a rendelés adataival.
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null) return NotFound();

            // admin mindent láthat
            if (User.IsInRole("Admin"))
                return Ok(order);

            // user csak a sajátját
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var clientId))
                return Unauthorized(new { message = "Érvénytelen token (nincs sub/clientId)." });

            if (order.ClientId != clientId)
                return Forbid();

            return Ok(order);
        }

        //Csak swagger tesztelésre. vagy manuális rendelésfelvételre használható, a frontend kosár->rendelés műveletet a CartController-ben valósítjuk meg.    
        [Authorize(Roles = "Admin")]//Ez a művelet létrehoz egy új rendelést a megadott adatok alapján. A CreatedAtAction választ ad vissza, amely tartalmazza az újonnan létrehozott rendelés helyét (GetById művelet) és az új rendelés adatait.
        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDTO dto)
        {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]//Ez a művelet töröl egy rendelést azonosító alapján. Ha a rendelés nem található, akkor NotFound választ adunk vissza, különben pedig NoContent választ, ami azt jelenti, hogy a művelet sikeresen végrehajtódott, de nincs visszaadandó tartalom.
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]//Ez a művelet frissít egy rendelés státuszát azonosító alapján. Ha a rendelés nem található vagy a státusz érvénytelen, akkor BadRequest választ adunk vissza, különben pedig NoContent választ, ami azt jelenti, hogy a művelet sikeresen végrehajtódott, de nincs visszaadandó tartalom.
        public async Task<IActionResult> UpdateStatus(int id, OrderStatusUpdateDTO dto)
        {
            var ok = await _service.UpdateStatusAsync(id, dto.NewStatus);
            if (!ok)
                return BadRequest(new { message = "Érvénytelen rendelés vagy státusz." });

            return NoContent();
        }

    }
}

