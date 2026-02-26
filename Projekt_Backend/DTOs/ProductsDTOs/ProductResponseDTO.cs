namespace Projekt_Backend.DTOs.ProductsDTOs
{ 
        public class ProductResponseDTO
        {
        //termékek lekérdezésére szolgáló DTO, a ProductService-ben használjuk
        public int ProductId { get; set; } //Az adatbázisban lévő termék egyedi azonosítója, amelyet a rendszer generál.
        public string ProductCode { get; set; } = string.Empty;//termék cikkszáma, egyedi azonosító a termékek között, amelyet a felhasználó ad meg létrehozáskor.
        public string ProductName { get; set; } = string.Empty;//termék neve, amelyet a felhasználó ad meg létrehozáskor.
        public int CategoryId { get; set; }//termék kategóriájának azonosítója, amely a termékhez tartozó kategória egyedi azonosítója, és amelyet a felhasználó ad meg létrehozáskor.
        public decimal NetPrice { get; set; }//termék nettó ára, amelyet a felhasználó ad meg létrehozáskor.
        public int UnitType { get; set; }//termék egységtípusa, amelyet a felhasználó ad meg létrehozáskor, és amely meghatározza, hogy a termék milyen mértékegységben van megadva (pl. darab, kilogramm, liter stb.).
        public string UnitTypeName { get; set; } = string.Empty;//termék egységtípusának neve, amelyet a rendszer határoz meg a UnitType érték alapján, és amely segít a felhasználónak megérteni, hogy milyen mértékegységben van megadva a termék (pl. darab, kilogramm, liter stb.).
        public string ProductGroup { get; set; } = string.Empty;//termékcsoport, amelyet a felhasználó ad meg létrehozáskor, és amely segít a termékek csoportosításában és szűrésében a későbbi műveletek során.

       
    }


}
