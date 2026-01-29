using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;
using WebshopConsole.Models;
using WebshopConsole.Services;

namespace WebshopConsole.Models
{
    public class WebshopContext : DbContext
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<cartItem> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server = tcp:jakobsdb.database.windows.net, 1433; Initial Catalog = JakobsDb; Persist Security Info = False; User ID = dbadmin; Password = Jakob1234; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30; ");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithOne()
                .HasForeignKey<Cart>(c => c.CustomerId);
            modelBuilder.Entity<cartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);
            modelBuilder.Entity<cartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
