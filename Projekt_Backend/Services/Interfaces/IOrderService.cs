using Projekt_Backend.DTOs.OrdersDTOs;

namespace Projekt_Backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderResponseDTO>> GetAllAsync(); // Rendelések + tételek + terméknév kiolvasása
        Task<OrderResponseDTO?> GetByIdAsync(int id);// Egy rendelés lekérdezése azonosító alapján, a OrderService-ben használjuk
        Task<OrderResponseDTO> CreateAsync(OrderCreateDTO dto);// Egy új rendelés létrehozása, a OrderService-ben használjuk

        Task<bool> UpdateStatusAsync(int orderId, string newStatus);// Egy rendelés státuszának frissítése azonosító alapján, a OrderService-ben használjuk
        Task<bool> DeleteAsync(int id);// Egy rendelés törlése azonosító alapján, a OrderService-ben használjuk
    }
}
