using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;


using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore;

namespace WebshopConsole.Services
{
    internal static class CartService
    {




        public static void AddToCart(Product product)
        {
           
            using var db = new WebshopContext();
            var customer = CustomerService.GetLoggedInCustomer(db);
            
            var cart = GetOrCreateCart(db, customer.Id);

            var existingItem = cart.Items
                .FirstOrDefault(i => i.ProductId == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Items.Add(new cartItem
                {
                    ProductId = product.Id,
                    Quantity = 1,
                    CartId = cart.Id
                });
            }
           
            
                db.SaveChanges();
           
        }
        
        public static Cart GetOrCreateCart(WebshopContext db, int customerId)
        {


            var cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.CustomerId == customerId);
            if (cart == null)
            {
                cart = new Cart { CustomerId = customerId };
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            return cart;
        }
        public static void ShowCart()
        {
            using var db = new WebshopContext();
            var cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.CustomerId == LoginService.LoggedInUser.Id);
            Console.Clear();

            if(cart == null || !cart.Items.Any())
            {
                Console.WriteLine("Din Kundvagn är tom");
                Console.ReadKey();
                return;
            }

            decimal total = 0;

            foreach(var item in cart.Items)
            {
                decimal price = item.Product.IsOnSale && item.Product.SalePrice.HasValue
                    ? item.Product.SalePrice.Value
                    : item.Product.Price;
                decimal sum = price * item.Quantity;
                total += sum;
                Console.WriteLine($"{item.Product.Name} x{item.Quantity} = {sum} kr");

            }
            Console.WriteLine("-----------------------");
            Console.WriteLine($"Totalt: {total} kr");
            Console.ReadKey();
        }

       // public static void ClearCart() => Cart.Clear();
    }
}
