namespace Projekt_Backend.DTOs.ProductsDTOs
{//termékek frissítésére szolgáló DTO, a ProductService-ben használjuk
    public class ProductUpdateDTO
    {
        public string ProductName { get; set; } = string.Empty;//termék neve, amelyet a felhasználó ad meg frissítéskor.
        public int CategoryId { get; set; }//termék kategóriájának azonosítója, amely a termékhez tartozó kategória egyedi azonosítója, és amelyet a felhasználó ad meg frissítéskor.
        public decimal NetPrice { get; set; }//termék nettó ára, amelyet a felhasználó ad meg frissítéskor.
        public int UnitType { get; set; }//termék egységtípusa, amelyet a felhasználó ad meg frissítéskor, és amely meghatározza, hogy a termék milyen mértékegységben van megadva (pl. darab, kilogramm, liter stb.).
        
        public string ProductGroup { get; set; } = string.Empty;

    }
}
