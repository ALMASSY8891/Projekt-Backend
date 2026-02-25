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

        public async Task<List<OrderResponseDTO>> GetAllAsync()
        {
            // Rendelések + tételek + terméknév kiolvasása
            var orders = await _db.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderResponseDTO
                {
                    OrderId = o.OrderId,
                    ClientId = o.ClientId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    Comment = o.Comment,
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
                        LineGross = (oi.UnitPrice * oi.Quantity) + ((oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m))
                    }).ToList()
                })
                .ToListAsync();

            // A teljes netto számítása (külön, mert Items már felépült)
            foreach (var o in orders)
            {
                o.TotalNet = o.Items.Sum(i => i.LineNet);
                o.TotalTax = o.Items.Sum(i => i.LineTax);
                o.TotalGross = o.Items.Sum(i => i.LineGross);
            }

            return orders;
        }
        // Egy rendelés lekérdezése azonosító alapján, a OrderService-ben használjuk
        public async Task<OrderResponseDTO?> GetByIdAsync(int id)
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
                        LineGross = (oi.UnitPrice * oi.Quantity) + ((oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m))

                    }).ToList()
                })
                .FirstOrDefaultAsync();
            // ha nincs ilyen id-jű rendelés, akkor null-t adunk vissza
            if (order == null) return null;
            // A teljes ár számítása (külön, mert Items már felépült)
            order.TotalNet = order.Items.Sum(i => i.LineNet);
            order.TotalTax = order.Items.Sum(i => i.LineTax);
            order.TotalGross = order.Items.Sum(i => i.LineGross);

            return order;
        }
        // Egy új rendelés létrehozása
        public async Task<OrderResponseDTO> CreateAsync(OrderCreateDTO dto)
        {
            // alap validáció (service szinten)
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("A rendelésnek legalább 1 tételt tartalmaznia kell.");

            if (dto.Items.Any(i => i.Quantity <= 0))
                throw new InvalidOperationException("A mennyiség nem lehet 0 vagy negatív.");

            // ellenőrzés: client létezik-e
            var clientExists = await _db.Clients.AnyAsync(c => c.ClientId == dto.ClientId);
            if (!clientExists)
                throw new InvalidOperationException("Nem létező ügyfél.");

            // betöltjük a termékeket (ár miatt)
            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();

            var products = await _db.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            if (products.Count != productIds.Count)
                throw new InvalidOperationException("A rendelésben szerepel nem létező termék.");

            //  Order entittás létrehozása – OrderDate: most, OrderStatus: "New"
            var order = new Order
            {
                ClientId = dto.ClientId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "New",
                Comment = dto.Comment?.Trim() ?? string.Empty
            };
            // előbb létre kell hozni a rendelést, hogy legyen OrderId-ja a FK-hoz
            _db.Orders.Add(order);
            await _db.SaveChangesAsync(); 

            //  OrderItem entity-k – UnitPrice a termék aktuális nettó ára
            foreach (var item in dto.Items)
            {
                //a megfelelő product megkeresése a listában, hogy megkapjuk az árát
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

            // visszaadjuk az elkészült rendelést
            var created = await GetByIdAsync(order.OrderId);
            if (created == null)
                throw new InvalidOperationException("A rendelés létrejött, de nem olvasható vissza.");

            return created;
        }
        public async Task<bool> UpdateStatusAsync(int orderId, string newStatus)
        {
            // alap ellenőrzés
            if (string.IsNullOrWhiteSpace(newStatus))
                return false;

            newStatus = newStatus.Trim();

            // engedélyezett státuszok (egyszerű, olvasható)
            if (newStatus != "New" &&
                newStatus != "Confirmed" &&
                newStatus != "Cancelled" &&
                newStatus != "Completed")
            {
                return false;
            }

            // rendelés betöltése
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return false;

            // státusz frissítése
            order.OrderStatus = newStatus;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // törlésnél figyelj: order_item FK-k miatt
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return false;

            // előbb tételek, aztán rendelés
            _db.OrderItems.RemoveRange(order.OrderItems);
            _db.Orders.Remove(order);// majd mentés

            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<List<OrderResponseDTO>> GetMyAsync(int clientId)
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
                        LineGross = (oi.UnitPrice * oi.Quantity) + ((oi.UnitPrice * oi.Quantity) * (oi.TaxRate / 100m))
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

