using Microsoft.EntityFrameworkCore;
using WebShopAPI.Model;
using WebShopAPI.Model.OrderModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebShopAPI.Data
{
    public class WebShopContext : DbContext
    {
        public WebShopContext(DbContextOptions<WebShopContext> options) : base(options)
        {
        }
        public DbSet<User> Useres { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=PetProject;User Id=sa;Password=SaraAttila1994;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30;");
        }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.User)
                .WithMany(u => u.OrderItems)
                .HasForeignKey(oi => oi.UserId);

            modelBuilder.Entity<Product>()
                .HasMany(p=> p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<Order>()
               .HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
        .        WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Product>()
               .Property(p => p.Discount)
               .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
               .Property(oi => oi.Price)
               .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Order>()
               .Property(o => o.TotalPrice)
               .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Product>()
               .Property(p => p.Price)
               .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<User>()
               .Property(u => u.Bonus)
               .HasColumnType("decimal(18, 2)");

        }
    }
}
