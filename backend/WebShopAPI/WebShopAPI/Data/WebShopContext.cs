using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebShopAPI.Model;
using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.UserModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebShopAPI.Data
{
    public class WebShopContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public WebShopContext(DbContextOptions<WebShopContext> options) : base(options)
        {
        }
        public DbSet<User> Useres { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                string connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Database connection string is not provided.");
                }

                optionsBuilder.UseSqlServer(connectionString);
            }
        }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            modelBuilder.Entity<User>()
               .HasOne(u => u.IdentityUser)
               .WithMany()
               .HasForeignKey(p => p.IdentityUserId)
               .IsRequired(false);
        

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.User)
                .WithMany(u => u.OrderItems)
                .HasForeignKey(oi => oi.UserId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<UserProfile>()
               .Property(u => u.Bonus)
               .HasColumnType("decimal(18, 2)");

        }
    }
}
