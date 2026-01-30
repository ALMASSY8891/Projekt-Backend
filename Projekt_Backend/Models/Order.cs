using System;
using System.Collections.Generic;

namespace Projekt_Backend.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int ClientId { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
