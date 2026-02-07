namespace Projekt_Backend.DTOs.OrdersDTOs
{
    // Egy rendelési tétel létrehozásához (termék + mennyiség)
    public class OrderItemCreateDTO
    {
        public int ProductId { get; set; }     // a termék azonosítója, amelyet a rendeléshez adunk hozzá.
        public int Quantity { get; set; }      // a rendelt mennyiség, amelyet a rendeléshez adunk hozzá.
    }
}
