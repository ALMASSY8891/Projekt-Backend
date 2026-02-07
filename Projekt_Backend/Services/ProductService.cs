using Microsoft.EntityFrameworkCore;
using Projekt_Backend.DTOs.ProductsDTOs;
using Projekt_Backend.Models;
using Projekt_Backend.Services.Interfaces;

namespace Projekt_Backend.Services
{
    // a ProductService felelős a termékekkel kapcsolatos üzleti logika megvalósításáért, és az IProductService interfészt valósítja meg
    public class ProductService : IProductService
    {
        // Az AcsolasContext példányát használjuk az adatbázis műveletekhez, amelyet a konstruktorban injektálunk be.
        private readonly AcsolasContext _db;
        // A konstruktorban beállítjuk az AcsolasContext példányát, amelyet a szolgáltatás használni fog az adatbázis műveletekhez.
        public ProductService(AcsolasContext db)
        {
            _db = db;
        }
        // Az alábbiakban megvalósítjuk az IProductService interfészben definiált metódusokat, amelyek a termékek CRUD műveleteit valósítják meg.
        public async Task<List<ProductResponseDTO>> GetAllAsync()//az összes termék lekérdezése, a ProductService-ben használjuk
        {
            return await _db.Products
                .AsNoTracking()
                .Select(p => new ProductResponseDTO
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    CategoryId = p.CategoryId,
                    NetPrice = p.NetPrice,
                    UnitType = p.UnitType
                })
                .ToListAsync();
        }
        //egy termék lekérdezése azonosító alapján, a ProductService-ben használjuk
        public async Task<ProductResponseDTO?> GetByIdAsync(int id)
        {
            return await _db.Products
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(p => new ProductResponseDTO
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    CategoryId = p.CategoryId,
                    NetPrice = p.NetPrice,
                    UnitType = p.UnitType
                })
                .FirstOrDefaultAsync();
        }
        //egy új termék létrehozása, a ProductService-ben használjuk
        public async Task<ProductResponseDTO> CreateAsync(ProductCreateDTO dto)
        {
            // Létrehozunk egy új Product entitást a DTO adataiból, és hozzáadjuk az adatbázishoz. Ezután elmentjük a változásokat, és visszaadjuk a létrehozott termék adatait egy ProductResponseDTO-ban.
            var product = new Product
            {
                ProductCode = dto.ProductCode.Trim(),
                ProductName = dto.ProductName.Trim(),
                CategoryId = dto.CategoryId,
                NetPrice = dto.NetPrice,
                UnitType = dto.UnitType
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return new ProductResponseDTO
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                NetPrice = product.NetPrice,
                UnitType = product.UnitType
            };
        }
        //egy meglévő termék frissítése azonosító alapján, a ProductService-ben használjuk
        public async Task<bool> UpdateAsync(int id, ProductUpdateDTO dto)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;

            product.ProductName = dto.ProductName.Trim();
            product.CategoryId = dto.CategoryId;
            product.NetPrice = dto.NetPrice;
            product.UnitType = dto.UnitType;

            await _db.SaveChangesAsync();
            return true;
        }
        //egy termék törlése azonosító alapján, a ProductService-ben használjuk
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

