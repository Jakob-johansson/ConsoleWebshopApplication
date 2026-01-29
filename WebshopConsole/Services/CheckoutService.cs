using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;

namespace WebshopConsole.Services
{
    internal class CheckoutService
    {
        public static void Checkout()
        {
            using var db = new WebshopContext();
            var customer = CustomerService.GetLoggedInCustomer(db);

            var cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.CustomerId == customer.Id);

            if(cart == null || !cart.Items.Any())
            {
                Console.WriteLine("Kundvagnen är tom.");
                Console.ReadKey();
                return;
            }
            
            foreach(var item in cart.Items)
            {
                if(item.Product.Stock < item.Quantity)
                {
                    Console.WriteLine($"Denna {item.Product.Name} är tyvvär slut i lager");
                    Console.ReadKey();

                    return;
                }
            }

            Console.Clear();

            //Priset på produkter 
            decimal productTotal = 0;

            foreach (var item in cart.Items)
            {
                decimal price = item.Product.IsOnSale && item.Product.SalePrice.HasValue
                    ? item.Product.SalePrice.Value
                    : item.Product.Price;

                productTotal += price * item.Quantity;
            }

            //Val av frakt
            Console.WriteLine("==============================================");
            Console.WriteLine("              VÄLJ FRAKTMETOD");
            Console.WriteLine("==============================================");
            Console.WriteLine("1. Standard (49 kr, 3–5 dagar)");
            Console.WriteLine("2. Express (99 kr, 1–2 dagar)");
            Console.WriteLine("3. Hämta i butik (0 kr, idag)");
            Console.Write("Välj (1-3): ");

            string shippingChoice = Console.ReadLine();

            decimal shippingCost;
            string shippingMethod;
            DateTime estimatedDelivery;

            switch (shippingChoice)
            {
                case "1":
                    shippingMethod = "Standard";
                    shippingCost = 49;
                    estimatedDelivery = DateTime.Now.AddDays(4);
                    break;
                case "2":
                    shippingMethod = "Express";
                    shippingCost = 99;
                    estimatedDelivery = DateTime.Now.AddDays(2);
                    break;
                case "3":
                    shippingMethod = "Hämta i butik";
                    shippingCost = 0;
                    estimatedDelivery = DateTime.Now;
                    break;
                default:
                    Console.WriteLine("Ogiltigt val.");
                    Console.ReadKey();
                    return;
            }

            decimal total = productTotal + shippingCost;

            //Checkout

            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine("                CHECKOUT");
            Console.WriteLine("==============================================");

            Console.WriteLine("KUNDUPPGIFTER");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"Namn:     {customer.FirstName} {customer.LastName}");
            Console.WriteLine($"Adress:  {customer.Address}, {customer.City}");
            Console.WriteLine($"Telefon: {customer.PhoneNumber}");

            Console.WriteLine("\nFRAKT");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"Metod: {shippingMethod}");
            Console.WriteLine($"Leveransdatum: {estimatedDelivery:yyyy-MM-dd}");
            Console.WriteLine($"Fraktkostnad: {shippingCost} kr");

            Console.WriteLine("\nPRODUKTER");
            Console.WriteLine("----------------------------------------------");

            foreach (var item in cart.Items)
            {
                decimal price = item.Product.IsOnSale && item.Product.SalePrice.HasValue
                    ? item.Product.SalePrice.Value
                    : item.Product.Price;

                Console.WriteLine($"{item.Product.Name} x{item.Quantity} - {price * item.Quantity} kr");
            }

            Console.WriteLine("\n----------------------------------------------");
            Console.WriteLine($"Produkter: {productTotal} kr");
            Console.WriteLine($"Frakt:     {shippingCost} kr");
            Console.WriteLine($"TOTALT:    {total} kr");
            Console.WriteLine("==============================================");

            Console.WriteLine("\nBekräfta köp? (J/N)");
            if (Console.ReadLine()?.ToUpper() != "J")
            {
                Console.WriteLine("Köpet avbröts.");
                Console.ReadKey();
                return;
            }

            //Betalning

            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine("            BETALNINGSMETOD");
            Console.WriteLine("==============================================");
            Console.WriteLine("1. Faktura");
            Console.WriteLine("2. Kort");
            Console.WriteLine("3. Swish");
            Console.Write("Välj (1-3): ");

            string paymentChoice = Console.ReadLine();
            string paymentMethod;

            switch (paymentChoice)
            {
                case "1": paymentMethod = "Faktura"; break;
                case "2": paymentMethod = "Kort"; break;
                case "3": paymentMethod = "Swish"; break;
                default:
                    Console.WriteLine("Ogiltigt val.");
                    Console.ReadKey();
                    return;
            }

            Console.WriteLine("\nBearbetar betalning...");
            Thread.Sleep(1500);

            //slutför köp

            foreach (var item in cart.Items)
            {
                item.Product.Stock -= item.Quantity;
            }

            db.CartItems.RemoveRange(cart.Items);
            db.SaveChanges();

            //kvitto
            Console.Clear();
            Console.WriteLine("==============================================");
            Console.WriteLine("     TACK SÅ MYCKET FÖR ATT DU HANDLAR HOS OSS");
            Console.WriteLine("==============================================");
            Console.WriteLine($"Betalningsmetod: {paymentMethod}");
            Console.WriteLine($"Frakt: {shippingMethod}");
            Console.WriteLine($"Totalt belopp: {total} kr");
            Console.WriteLine($"Leveransdatum: {estimatedDelivery:yyyy-MM-dd}");
            Console.WriteLine("\nVi hoppas att du handlar hos oss igen");
            Console.WriteLine("==============================================");
            Console.ReadKey();
        }

    }
}
