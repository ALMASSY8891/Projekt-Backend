using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt_Backend.DTOs.ProductsDTOs;
using Projekt_Backend.Services.Interfaces;

namespace Projekt_Backend.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        //DI:a controller csak az interfacet ismeri, nem a konkrét implementációt, így könnyen cserélhető a szolgáltatás implementációja anélkül, hogy a controller kódját módosítani kellene.
        public ProductsController(IProductService service)
        {
            _service = service;
        }

        
        // Az alábbiakban megvalósítjuk a termékekkel kapcsolatos HTTP műveleteket, amelyek a ProductService-ben definiált metódusokat hívják meg a megfelelő műveletek végrehajtásához.
        [HttpGet]//Ez a művelet lekéri az összes terméket. Az async/await használata lehetővé teszi, hogy a művelet aszinkron módon fusson, így nem blokkolja a szerver erőforrásait, amíg az adatbázisból lekéri a termékeket.
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }
        [Authorize]
        [HttpGet("{id}")]//Ez a művelet lekéri egy adott terméket azonosító alapján. Ha a termék nem található, akkor NotFound választ adunk vissza, különben pedig az Ok választ a termék adataival.
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]//Ez a művelet létrehoz egy új terméket a megadott adatok alapján. A CreatedAtAction választ ad vissza, amely tartalmazza az újonnan létrehozott termék helyét (GetById művelet) és az új termék adatait.
        public async Task<IActionResult> Create(ProductCreateDTO dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
        }

        [HttpPut("{id}")]//Ez a művelet frissít egy meglévő terméket azonosító alapján. Ha a termék nem található, akkor NotFound választ adunk vissza, különben pedig NoContent választ, ami azt jelenti, hogy a művelet sikeresen végrehajtódott, de nincs visszaadandó tartalom.
        public async Task<IActionResult> Update(int id, ProductUpdateDTO dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]//Ez a művelet töröl egy terméket azonosító alapján. Ha a termék nem található, akkor NotFound választ adunk vissza, különben pedig NoContent választ, ami azt jelenti, hogy a művelet sikeresen végrehajtódott, de nincs visszaadandó tartalom.
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

