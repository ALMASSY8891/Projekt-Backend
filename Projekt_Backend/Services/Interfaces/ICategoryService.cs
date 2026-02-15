using Projekt_Backend.DTOs.CategoriesDTOs;

namespace Projekt_Backend.Services.Interfaces
{
  
    // megmondja, milyen műveleteket KELL tudnia a CategoryService-nek
    public interface ICategoryService
    {
        // Összes kategória lekérdezése
        Task<List<CategoryResponseDTO>> GetAllAsync();

        // Egy kategória lekérdezése ID alapján
        Task<CategoryResponseDTO?> GetByIdAsync(int id);

        // Új kategória létrehozása
        Task<CategoryResponseDTO> CreateAsync(CategoryCreateDTO dto);

        // Kategória törlése ID alapján
        Task<bool> DeleteAsync(int id);
    }
}

