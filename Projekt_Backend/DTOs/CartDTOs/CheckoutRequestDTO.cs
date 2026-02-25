using System.Collections.Generic;
using Projekt_Backend.DTOs.OrdersDTOs;

namespace Projekt_Backend.DTOs.CartDTOs
{
    // Checkout kérés: a frontend kosár tartalma + opcionális megjegyzés
    // ClientId NEM jön a DTO-ból, hanem JWT-ből olvassuk ki a controllerben.
    public class CheckoutRequestDTO
    {
        public string Comment { get; set; } = string.Empty;

        // Ugyanazt a tétel DTO-t használjuk, mint a rendelésnél (ProductId + Quantity)
        public List<OrderItemCreateDTO> Items { get; set; } = new();
    }
}
