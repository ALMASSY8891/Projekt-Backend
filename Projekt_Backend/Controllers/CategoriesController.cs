using Microsoft.AspNetCore.Mvc;
using Projekt_Backend.DTOs.CategoriesDTOs;
using Projekt_Backend.Services.Interfaces;

namespace Projekt_Backend.Controllers
{
    [ApiController]                
    [Route("api/categories")]        
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        // DI – a service-t kapja meg
        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        // GET /api/categories 
        [HttpGet]// Minden kategória lekérése
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        // GET /api/categories/{id}
        [HttpGet("{id}")]// Egy kategória lekérése id alapján
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _service.GetByIdAsync(id);
            if (cat == null) return NotFound();
            return Ok(cat);
        }

        // POST /api/categories
        [HttpPost]// Új kategória létrehozása
        public async Task<IActionResult> Create(CategoryCreateDTO dto)
        {
            var created = await _service.CreateAsync(dto);

            // 201 + Location header
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.CategoryId },
                created
            );
        }

        // DELETE /api/categories/{id}
        [HttpDelete("{id}")]// Egy kategória törlése id alapján
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

