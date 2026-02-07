using Microsoft.AspNetCore.Mvc;
using Projekt_Backend.DTOs.OrdersDTOs;
using Projekt_Backend.Services.Interfaces;

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

        [HttpGet]//Ez a művelet lekéri az összes rendelést. Az async/await használata lehetővé teszi, hogy a művelet aszinkron módon fusson, így nem blokkolja a szerver erőforrásait, amíg az adatbázisból lekéri a rendeléseket.
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]//Ez a művelet lekéri egy adott rendelést azonosító alapján. Ha a rendelés nem található, akkor NotFound választ adunk vissza, különben pedig az Ok választ a rendelés adataival.
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]//Ez a művelet létrehoz egy új rendelést a megadott adatok alapján. A CreatedAtAction választ ad vissza, amely tartalmazza az újonnan létrehozott rendelés helyét (GetById művelet) és az új rendelés adatait.
        public async Task<IActionResult> Create(OrderCreateDTO dto)
        {
            // itt később authból jön a ClientId
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
        }

        [HttpDelete("{id}")]//Ez a művelet töröl egy rendelést azonosító alapján. Ha a rendelés nem található, akkor NotFound választ adunk vissza, különben pedig NoContent választ, ami azt jelenti, hogy a művelet sikeresen végrehajtódott, de nincs visszaadandó tartalom.
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
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

