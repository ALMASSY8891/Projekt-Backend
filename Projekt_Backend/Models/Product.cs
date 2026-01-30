using System;
using System.Collections.Generic;

namespace Projekt_Backend.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductCode { get; set; } = null!;

    public int CategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public int UnitType { get; set; }

    public decimal NetPrice { get; set; }

    public virtual Category Category { get; set; } = null!;
}
