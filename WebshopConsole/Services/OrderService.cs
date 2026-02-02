using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;

namespace WebshopConsole.Services
{
    internal class OrderService
    {
        public static void ShowMyOrders()
        {
            using var db = new WebshopContext();
            var customer = CustomerService.GetLoggedInCustomer(db);

            var orders = db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.CustomerId == customer.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            Console.Clear();

            if (!orders.Any())
            {
                Console.WriteLine("Du har inga tidigare ordrar.");
                Console.ReadKey();
                return;
            }

            foreach (var order in orders)
            {
                Console.WriteLine("================================");
                Console.WriteLine($"Order #{order.Id}");
                Console.WriteLine($"Datum: {order.OrderDate}");
                Console.WriteLine($"Status: {order.Status}");
                Console.WriteLine("--------------------------------");

                foreach (var item in order.Items)
                {
                    Console.WriteLine($"{item.Product.Name} x{item.Quantity} - {item.PriceAtPurchase * item.Quantity} kr");
                }

                Console.WriteLine($"Totalt: {order.TotalPrice} kr");
                Console.WriteLine("================================\n");
            }

            Console.ReadKey();
        }
        public static void ShowAllOrders()
        {
            using var db = new WebshopContext();

            var orders = db.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            Console.Clear();

            foreach (var order in orders)
            {
                Console.WriteLine($"Order #{order.Id} | Kund: {order.Customer.FirstName} | Status: {order.Status} | {order.TotalPrice} kr");
            }

            Console.Write("\nAnge OrderID för att avbryta (0 för tillbaka): ");
            if (!int.TryParse(Console.ReadLine(), out int orderId) || orderId == 0)
                return;

            var orderToCancel = db.Orders
                 .Include(o => o.Items)
                 .ThenInclude(i => i.Product)
                 .FirstOrDefault(o => o.Id == orderId);

            if(orderToCancel == null)
            {
                Console.WriteLine("Ordern med det ID:et hittades inte. ");
                Console.ReadKey();
                return;

            }
            if(orderToCancel.Status == "Avbruten")
            {
                Console.WriteLine("Ordern är redan avbruten. ");
                Console.ReadKey();
                return;
            }
            foreach(var item in orderToCancel.Items)
            {
                item.Product.Stock += item.Quantity;
            }
            orderToCancel.Status = "Avbruten";
            db.SaveChanges();

            Console.WriteLine("Orden har avbrutits och produkterna är tillbaka i lager. ");
            Console.ReadKey();



        }
    }
}

