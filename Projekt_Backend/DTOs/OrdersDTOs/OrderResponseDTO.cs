using System;
using System.Collections.Generic;
using Projekt_Backend.DTOs.OrdersDTOs;

namespace Projekt_Backend.DTOs.OrdersDTOs
{
    // Rendelés visszaadása
    public class OrderResponseDTO
    {
        public int OrderId { get; set; } // A rendelés egyedi azonosítója, amelyet a rendszer generál.
        public int ClientId { get; set; } // A rendelést leadó ügyfél azonosítója, amely a rendeléshez tartozó ügyfél egyedi azonosítója, és amelyet a felhasználó ad meg létrehozáskor.

        public DateTime OrderDate { get; set; } // A rendelés leadásának dátuma és időpontja, amelyet a rendszer generál a rendelés létrehozásakor.
        public string OrderStatus { get; set; } = string.Empty;// A rendelés státusza, amely a rendelés aktuális állapotát jelzi (pl. "Folyamatban", "Teljesítve", "Törölve" stb.), és amelyet a rendszer vagy a felhasználó frissíthet a rendelés feldolgozása során.
        public string Comment { get; set; } = string.Empty;// A rendeléshez fűzött megjegyzés, amelyet a felhasználó adhat meg létrehozáskor vagy frissítéskor, és amely tartalmazhat további információkat a rendelésről (pl. szállítási utasítások, különleges kérések stb.).

        public decimal TotalNet { get; set; } //A teljes netto összeg a tételekből számolva
        public decimal TotalTax { get; set; }     // összes ÁFA
        public decimal TotalGross { get; set; }   // bruttó végösszeg

        public string ClientEmail { get; set; } = string.Empty; // A rendelést leadó ügyfél email címe, amely a rendeléshez tartozó ügyfél egyedi email címe, és amelyet a rendszer vagy a felhasználó adhat meg létrehozáskor vagy frissítéskor.
        public string ClientName { get; set; } = string.Empty;// A rendelést leadó ügyfél neve, amely a rendeléshez tartozó ügyfél egyedi neve, és amelyet a rendszer vagy a felhasználó adhat meg létrehozáskor vagy frissítéskor.
        public List<OrderItemResponseDTO> Items { get; set; } = new();// A rendelés listája, amely a rendeléshez tartozó tételeket tartalmazza, és amelyeket a rendszer vagy a felhasználó adhat meg létrehozáskor vagy frissítéskor. Minden tétel tartalmazza a termék azonosítóját, nevét, mennyiségét, egységárát és adókulcsát.
    }
}


