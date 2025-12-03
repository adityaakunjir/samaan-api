using Microsoft.EntityFrameworkCore;
using Samaan.API.Models;

namespace Samaan.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Merchant
            modelBuilder.Entity<Merchant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithOne(u => u.Merchant)
                      .HasForeignKey<Merchant>(e => e.UserId);
            });

            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Merchant)
                      .WithMany(m => m.Products)
                      .HasForeignKey(e => e.MerchantId);
            });

            // Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Customer)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.Merchant)
                      .WithMany(m => m.Orders)
                      .HasForeignKey(e => e.MerchantId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.Items)
                      .HasForeignKey(e => e.OrderId);
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}