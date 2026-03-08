using Microsoft.EntityFrameworkCore;
using Projekt_Backend.DTOs.OrdersDTOs;
using Projekt_Backend.Models;
using Projekt_Backend.Services.Interfaces;

namespace Projekt_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly AcsolasContext _db;

        public OrderService(AcsolasContext db)
        {
            _db = db;
        }

        public async Task<List<OrderResponseDTO>> GetAllAsync() // Minden rendelés lekérése
        {
            return await _db.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderResponseDTO
                {
                    OrderId = o.OrderId,
                    ClientId = o.ClientId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    Comment = o.Comment,

                    TotalNet = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                    TotalTax = o.OrderItems.Sum(oi => (oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m)),
                    TotalGross = o.OrderItems.Sum(oi =>
                        (oi.UnitPrice * oi.Quantity) +
                        ((oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m))
                    ),

                    ClientEmail = o.Client.Email,
                    ClientName = o.Client.Name,

                    Items = o.OrderItems.Select(oi => new OrderItemResponseDTO
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName,
                        Quantity = oi.Quantity,
                        TaxRate = oi.TaxRate,
                        UnitPrice = oi.UnitPrice,
                        LineNet = oi.UnitPrice * oi.Quantity,
                        LineTax = (oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m),
                        LineGross =
                            (oi.UnitPrice * oi.Quantity) +
                            ((oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m))
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderResponseDTO?> GetByIdAsync(int id) // Egy rendelés lekérése id alapján
        {
            var order = await _db.Orders
                .AsNoTracking()
                .Where(o => o.OrderId == id)
                .Select(o => new OrderResponseDTO
                {
                    OrderId = o.OrderId,
                    ClientId = o.ClientId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    Comment = o.Comment,

                    ClientEmail = o.Client.Email,
                    ClientName = o.Client.Name,

                    Items = o.OrderItems.Select(oi => new OrderItemResponseDTO
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName,
                        Quantity = oi.Quantity,
                        TaxRate = oi.TaxRate,
                        UnitPrice = oi.UnitPrice,
                        LineNet = oi.UnitPrice * oi.Quantity,
                        LineTax = (oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m),
                        LineGross =
                            (oi.UnitPrice * oi.Quantity) +
                            ((oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m))
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
                return null;

            order.TotalNet = order.Items.Sum(i => i.LineNet);
            order.TotalTax = order.Items.Sum(i => i.LineTax);
            order.TotalGross = order.Items.Sum(i => i.LineGross);

            return order;
        }

        public async Task<OrderResponseDTO> CreateAsync(OrderCreateDTO dto) // Új rendelés létrehozása
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("A rendelésnek legalább 1 tételt tartalmaznia kell.");

            if (dto.Items.Any(i => i.Quantity <= 0))
                throw new InvalidOperationException("A mennyiség nem lehet 0 vagy negatív.");

            var clientExists = await _db.Clients.AnyAsync(c => c.ClientId == dto.ClientId);
            if (!clientExists)
                throw new InvalidOperationException("Nem létező ügyfél.");

            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();

            var products = await _db.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            if (products.Count != productIds.Count)
                throw new InvalidOperationException("A rendelésben szerepel nem létező termék.");

            var order = new Order
            {
                ClientId = dto.ClientId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "New",
                Comment = dto.Comment?.Trim() ?? string.Empty
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.ProductId == item.ProductId);

                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.NetPrice,
                    TaxRate = 27
                };

                _db.OrderItems.Add(orderItem);
            }

            await _db.SaveChangesAsync();

            var created = await GetByIdAsync(order.OrderId);
            if (created == null)
                throw new InvalidOperationException("A rendelés létrejött, de nem olvasható vissza.");

            return created;
        }

        public async Task<bool> UpdateStatusAsync(int orderId, string newStatus) // Rendelés státuszának frissítése
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                return false;

            newStatus = newStatus.Trim();

            if (newStatus != "New" &&
                newStatus != "Confirmed" &&
                newStatus != "Cancelled" &&
                newStatus != "Completed")
            {
                return false;
            }

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return false;

            order.OrderStatus = newStatus;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return false;

            _db.OrderItems.RemoveRange(order.OrderItems);
            _db.Orders.Remove(order);

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrderResponseDTO>> GetMyAsync(int clientId) // Az adott ügyfél rendeléseinek lekérése
        {
            var orders = await _db.Orders
                .AsNoTracking()
                .Where(o => o.ClientId == clientId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderResponseDTO
                {
                    OrderId = o.OrderId,
                    ClientId = o.ClientId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    Comment = o.Comment,

                    ClientEmail = o.Client.Email,
                    ClientName = o.Client.Name,

                    Items = o.OrderItems.Select(oi => new OrderItemResponseDTO
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.ProductName,
                        Quantity = oi.Quantity,
                        TaxRate = oi.TaxRate,
                        UnitPrice = oi.UnitPrice,
                        LineNet = oi.UnitPrice * oi.Quantity,
                        LineTax = (oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m),
                        LineGross =
                            (oi.UnitPrice * oi.Quantity) +
                            ((oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m))
                    }).ToList()
                })
                .ToListAsync();

            foreach (var o in orders)
            {
                o.TotalNet = o.Items.Sum(i => i.LineNet);
                o.TotalTax = o.Items.Sum(i => i.LineTax);
                o.TotalGross = o.Items.Sum(i => i.LineGross);
            }

            return orders;
        }
    }
}