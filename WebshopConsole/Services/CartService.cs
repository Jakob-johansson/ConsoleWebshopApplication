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
        public static void RemoveOneFromCart(int productId)
        {
            using var db = new WebshopContext();
            var customer = CustomerService.GetLoggedInCustomer(db);

            var cart = db.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if (cart == null)
                return;

            var item = cart.Items
                .FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                return;

            item.Quantity--;

            if (item.Quantity <= 0)
            {
                db.CartItems.Remove(item);
            }

            db.SaveChanges();
        }
        //Visar kundens kundvagn
        public static void ShowCart()
        {
            using var db = new WebshopContext();
            var customer = CustomerService.GetLoggedInCustomer(db);

            var cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            Console.Clear();

            if (cart == null || !cart.Items.Any())
            {
                Console.WriteLine("Din kundvagn är tom");
                Console.ReadKey();
                return;
            }

            decimal total = 0;

            foreach (var item in cart.Items)
            {
                decimal price = item.Product.IsOnSale && item.Product.SalePrice.HasValue
                    ? item.Product.SalePrice.Value
                    : item.Product.Price;

                decimal sum = price * item.Quantity;
                total += sum;

                Console.WriteLine(
                    $"ID: {item.Product.Id} | {item.Product.Name} x{item.Quantity} = {sum} kr"
                );
            }

            Console.WriteLine("-----------------------");
            Console.WriteLine($"Totalt: {total} kr");
            Console.WriteLine();
            Console.WriteLine("Ange ProduktID för att ändra antal: ");
            //Köp
            Console.WriteLine("\n 1. Slutför köp \n 0. tillbaka");
            var buyChoice = Console.ReadLine();

            if (buyChoice == "1")
            {
                CheckoutService.Checkout();
            }
            if (!int.TryParse(Console.ReadLine(), out int productId) || productId == 0)
                return;

            var selectedItem = cart.Items
                .FirstOrDefault(i => i.Product.Id == productId);

            if (selectedItem == null)
            {
                Console.WriteLine("Produkten finns inte i kundkorgen.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("1. Lägg till 1 st");
            Console.WriteLine("2. Ta bort 1 st");

            var choice = Console.ReadLine();

            if (choice == "1")
                AddToCart(selectedItem.Product);
            else if (choice == "2")
                RemoveOneFromCart(productId);

           
            


        }



    }
}
