namespace Projekt_Backend.DTOs.OrdersDTOs
{
    // Egy tétel visszaadása (beleértve a termék nevét is, hogy ne kelljen külön lekérdezni)
    public class OrderItemResponseDTO
    {
        public int OrderItemId { get; set; }// a rendelési tétel azonosítója, amely egyedi módon azonosítja a rendelés egy adott tételét.
        public int ProductId { get; set; }// a termék azonosítója, amelyet a rendeléshez adunk hozzá.
        public string ProductName { get; set; } = string.Empty;// a termék neve, amelyet a rendeléshez adunk hozzá, hogy ne kelljen külön lekérdezni a termék nevét.
        public int Quantity { get; set; }// a rendelt mennyiség, amelyet a rendeléshez adunk hozzá.
        public int TaxRate { get; set; }       // ÁFA kulcs, amely a termékhez tartozó adókulcsot jelöli, és amelyet a rendeléshez adunk hozzá. A DB-ben int
        public decimal UnitPrice { get; set; } // Egységár.DB-ben unit_price
        public decimal LineNet { get; set; }      // nettó összesen
        public decimal LineTax { get; set; }      // ÁFA összege
        public decimal LineGross { get; set; }    // bruttó összesen
    }
}
