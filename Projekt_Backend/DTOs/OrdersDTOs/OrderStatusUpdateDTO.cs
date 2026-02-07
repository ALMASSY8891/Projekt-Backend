namespace Projekt_Backend.DTOs.OrdersDTOs
{
    public class OrderStatusUpdateDTO // Ez a DTO osztály az OrderController-ben használatos a rendelés státuszának frissítéséhez, és csak egy új státusz értékét tartalmazza
    {
        public string NewStatus { get; set; } = string.Empty;// A rendelés új státusza, amely a rendelés aktuális állapotát jelzi (pl. "Folyamatban", "Teljesítve", "Törölve" stb.).
    }
}
