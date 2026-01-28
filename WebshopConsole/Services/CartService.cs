using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;

using WebshopConsole.Models;

namespace WebshopConsole.Services
{
    internal static class CartService
    {
        private static readonly List<cartItem> Cart = new();

        public static void AddToCart(Product product)
        {
            var existing = Cart.FirstOrDefault(c => c.Product.Id == product.Id);

            if (existing != null)
                existing.Quantity++;
            else
                Cart.Add(new cartItem { Product = product, Quantity = 1 });

            Console.WriteLine($"{product.Name} lades i kundkorgen!");
            Console.ReadKey();
        }

        public static void ShowCart()
        {
            Console.Clear();
            Console.WriteLine("DIN KUNDKORG\n");

            decimal total = 0;

            foreach (var item in Cart)
            {
                decimal sum = item.Product.Price * item.Quantity;
                total += sum;

                Console.WriteLine($"{item.Product.Name} x{item.Quantity} - {sum} kr");
            }

            Console.WriteLine("\n--------------------");
            Console.WriteLine($"Totalt: {total} kr");
            Console.ReadKey();
        }

        public static void ClearCart() => Cart.Clear();
    }
}
