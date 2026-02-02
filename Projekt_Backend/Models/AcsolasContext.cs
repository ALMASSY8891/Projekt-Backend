using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Projekt_Backend.Models;

public partial class AcsolasContext : DbContext
{
    public AcsolasContext()
    {
    }

    public AcsolasContext(DbContextOptions<AcsolasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<AuthoritySession> AuthoritySessions { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;database=acsolas;user=root;password=;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PRIMARY");

            entity.ToTable("appointment");

            entity.HasIndex(e => e.ClientId, "Client_Id");

            entity.Property(e => e.AppointmentId)
                .HasColumnType("int(11)")
                .HasColumnName("Appointment_Id");
            entity.Property(e => e.ClientId)
                .HasColumnType("int(11)")
                .HasColumnName("Client_Id");
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("End_Time");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("Start_Time");
            entity.Property(e => e.Status).HasColumnType("int(11)");

            entity.HasOne(d => d.Client).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("appointment_ibfk_1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("category");

            entity.HasIndex(e => e.CategoryName, "Category_Name");

            entity.Property(e => e.CategoryId)
                .HasColumnType("int(11)")
                .HasColumnName("Category_Id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(40)
                .HasColumnName("Category_Name");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PRIMARY");

            entity.ToTable("client");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.ClientId)
                .HasColumnType("int(11)")
                .HasColumnName("Client_Id");
            entity.Property(e => e.BillingAddress)
                .HasMaxLength(50)
                .HasColumnName("Billing_Address");
            entity.Property(e => e.Email).HasMaxLength(40);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Telephone).HasMaxLength(40);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("order");

            entity.HasIndex(e => e.ClientId, "Client_Id");

            entity.HasIndex(e => new { e.OrderId, e.ClientId }, "Order_Id");

            entity.Property(e => e.OrderId)
                .HasColumnType("int(40)")
                .HasColumnName("Order_Id");
            entity.Property(e => e.ClientId)
                .HasColumnType("int(11)")
                .HasColumnName("Client_Id");
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("Order_Date");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(20)
                .HasColumnName("Order_Status");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_ibfk_1");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PRIMARY");

            entity.ToTable("order_item");

            entity.HasIndex(e => e.OrderId, "Order_Id");

            entity.HasIndex(e => e.ProductId, "fk_order_item_product");

            entity.Property(e => e.OrderItemId)
                .HasColumnType("int(11)")
                .HasColumnName("Order_Item_Id");
            entity.Property(e => e.OrderId)
                .HasColumnType("int(11)")
                .HasColumnName("Order_Id");
            entity.Property(e => e.ProductId)
                .HasColumnType("int(11)")
                .HasColumnName("Product_Id");
            entity.Property(e => e.Quantity).HasColumnType("int(11)");
            entity.Property(e => e.TaxRate)
                .HasColumnType("int(11)")
                .HasColumnName("Tax_Rate");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(11)
                .HasColumnName("Unit_Price");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("order_item_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_order_item_product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("product");

            entity.HasIndex(e => e.CategoryId, "Category_Id");

            entity.HasIndex(e => e.ProductCode, "Product_Code");

            entity.Property(e => e.ProductId)
                .HasColumnType("int(11)")
                .HasColumnName("Product_Id");
            entity.Property(e => e.CategoryId)
                .HasColumnType("int(11)")
                .HasColumnName("Category_Id");
            entity.Property(e => e.NetPrice)
                .HasPrecision(11)
                .HasColumnName("Net_Price");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(30)
                .HasColumnName("Product_Code");
            entity.Property(e => e.ProductName)
                .HasMaxLength(30)
                .HasColumnName("Product_Name");
            entity.Property(e => e.UnitType)
                .HasColumnType("int(10)")
                .HasColumnName("Unit_Type");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("product_ibfk_1");
        });
        modelBuilder.Entity<AuthoritySession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("authority_session");

            entity.HasIndex(e => e.Token, "uq_authority_session_token").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ClientId)
                .HasColumnType("int(11)")
                .HasColumnName("Client_Id");

            entity.Property(e => e.Token).HasMaxLength(64);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_At");

            entity.Property(e => e.IsRevoked)
                .HasColumnType("tinyint(1)")
                .HasColumnName("Is_Revoked");

            entity.HasOne(d => d.Client)
                .WithMany() // nem kell ICollection<ClientSession> a Clientben
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_authority_session_client");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
