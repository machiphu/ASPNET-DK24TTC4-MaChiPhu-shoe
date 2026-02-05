using MaChiPhuShoe.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaChiPhuShoe.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ====== Các DbSet ======
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // ====== Fluent API ======
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // ⚠️ giữ để Identity tạo bảng AspNet*

            // Order – OrderDetail (1-n)
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product – OrderDetail (1-n)
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category – Product (1-n)
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products!)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Table name mapping (optional, rõ ràng hơn)
            builder.Entity<Product>().ToTable("Products");
            builder.Entity<Category>().ToTable("Categories");
            builder.Entity<Order>().ToTable("Orders");
            builder.Entity<OrderDetail>().ToTable("OrderDetails");
        }
    }
}
