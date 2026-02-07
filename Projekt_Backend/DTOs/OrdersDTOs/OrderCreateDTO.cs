using System.Collections.Generic;
using Projekt_Backend.DTOs.OrdersDTOs;

namespace Projekt_Backend.DTOs.OrdersDTOs
{
    // Rendelés létrehozásához szükséges adatok (ügyfél + megjegyzés + tételek)
    public class OrderCreateDTO
    {
        public int ClientId { get; set; }  // jwt-ből is kinyerhető, de így egyszerűbb a tesztelés
        public string Comment { get; set; } = string.Empty;// rendeléshez fűzött megjegyzés, amelyet a felhasználó ad meg létrehozáskor.

        public List<OrderItemCreateDTO> Items { get; set; } = new();// rendelés tételei, amelyek a rendeléshez tartozó termékeket és azok mennyiségét tartalmazzák, és amelyeket a felhasználó ad meg létrehozáskor.
    }
}

