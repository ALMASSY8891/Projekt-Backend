using Microsoft.EntityFrameworkCore;                    // EF Core műveletekhez
using Projekt_Backend.DTOs.CategoriesDTOs;              // DTO-k
using Projekt_Backend.Models;                           // Entity-k
using Projekt_Backend.Services.Interfaces;              // Interface

namespace Projekt_Backend.Services
{
    // Ez az osztály végzi a tényleges munkát az adatbázissal
    public class CategoryService : ICategoryService
    {
        // Adatbázis context (EF Core)
        private readonly AcsolasContext _db;

        // Konstruktor – Dependency Injection
        public CategoryService(AcsolasContext db)
        {
            _db = db;
        }

        // Összes kategória lekérdezése
        public async Task<List<CategoryResponseDTO>> GetAllAsync()
        {
            return await _db.Categories
                .AsNoTracking()               // csak olvasunk, nem módosítunk
                .OrderBy(c => c.CategoryName) // ABC sorrend
                .Select(c => new CategoryResponseDTO
                {
                    // Entity → DTO leképezés
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
        }

        // Egy kategória lekérdezése ID alapján
        public async Task<CategoryResponseDTO?> GetByIdAsync(int id)
        {
            return await _db.Categories
                .AsNoTracking()
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryResponseDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .FirstOrDefaultAsync(); // ha nincs ilyen ID → null
        }

        // Új kategória létrehozása
        public async Task<CategoryResponseDTO> CreateAsync(CategoryCreateDTO dto)
        {
            // Védjük magunkat az üres név ellen
            var name = (dto.CategoryName ?? string.Empty).Trim();
            if (name.Length == 0)
                throw new InvalidOperationException("Category name is required.");

            // Entity létrehozása
            var category = new Category
            {
                CategoryName = name
            };

            // Mentés adatbázisba
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            // Visszaadjuk DTO-ként
            return new CategoryResponseDTO
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }

        // Kategória törlése
        public async Task<bool> DeleteAsync(int id)
        {
            // Megkeressük az entity-t
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
                return false;

            // Törlés
            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}


