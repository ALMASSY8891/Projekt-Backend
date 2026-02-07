using Projekt_Backend.DTOs.ProductsDTOs;

namespace Projekt_Backend.Services.Interfaces
{// mit tud a ProductService
    public interface IProductService
    {
        // CRUD műveletek a Product entitásra
        Task<List<ProductResponseDTO>> GetAllAsync();//az összes termék lekérdezése, a ProductService-ben használjuk
        Task<ProductResponseDTO?> GetByIdAsync(int id);//egy termék lekérdezése azonosító alapján, a ProductService-ben használjuk
        Task<ProductResponseDTO> CreateAsync(ProductCreateDTO dto);//egy új termék létrehozása, a ProductService-ben használjuk
        Task<bool> UpdateAsync(int id, ProductUpdateDTO dto);//egy meglévő termék frissítése azonosító alapján, a ProductService-ben használjuk
        Task<bool> DeleteAsync(int id);//egy termék törlése azonosító alapján, a ProductService-ben használjuk
    }
}

