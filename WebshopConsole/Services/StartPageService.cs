using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;
namespace WebshopConsole.Services
{
    internal class StartPageService
    {
        public static string ShowStartPage()
        {
            
                Console.Clear();

                using var db = new WebshopContext();
                
                var products = db.Products;
                var campaignProducts = db.Products
                    .Where(p => p.IsOnSale)
                    .Take(3)
                    .ToList();




                DrawService.DrawBox(0, 0, 120, 3);
            Console.SetCursorPosition(41,1);
            Console.WriteLine("====== Jakobs Kläder ======");
                DrawService.DrawBox(0, 4, 65, 6, "  KAMPANJ!!!  ");
                DrawService.DrawBox(80, 4, 20, 3, "  Användare  ");
            if (LoginService.IsLoggedIn && LoginService.LoggedInUser != null)
            {
                Console.SetCursorPosition(81, 5);
                Console.WriteLine(LoginService.LoggedInUser.Username);
            }
            int colWidth = 20;
                int startX = 5;
                int startY = 7;

                int col = 2;
            
                foreach (var p in campaignProducts)
                {
                    Console.SetCursorPosition(col, 5);
                    Console.Write($"{p.Name} ID:[{p.Id}]");
                    Console.SetCursorPosition(col, 6);
                    Console.Write($"Färg: {p.Color}");
                    Console.SetCursorPosition(col, 7);
                    Console.Write($"Strl: {p.Size}");
                    Console.SetCursorPosition(col, 8);
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write($"{p.SalePrice} kr");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($" ~{p.Price}~");
                    Console.ResetColor();

                    col += 20;
                }
            Console.SetCursorPosition(0, 10);
            Console.WriteLine("För att lägga till en kampanj vara i kungkorgen ange ID: ");
               
                DrawService.DrawBox(0, 11, 90, 9);

                Console.SetCursorPosition(2, 12);
                Console.Write("Välkommen!");

                Console.SetCursorPosition(2, 13);
                Console.Write("Välj ett alternativ nedan:");

                Console.SetCursorPosition(2, 14);
                Console.Write("A. Gå till webshoppen");
            if(LoginService.LoggedInUser == null)
            {
                Console.SetCursorPosition(2, 15);
                Console.Write("B. Logga in");

                Console.SetCursorPosition(2, 16);
                Console.Write("C. Registrera");
               
            }
            else
            { 
                Console.SetCursorPosition(2, 15);
                Console.WriteLine("B. Kundkorg");

                Console.SetCursorPosition(2, 16);
                Console.Write("C. Konto");
                Console.SetCursorPosition(2, 17);
                Console.Write("D. Logga ut");
            }
                

               
            if (LoginService.AdminOnline)
            {
                Console.SetCursorPosition(2, 17);
                Console.Write("D. Adminpanel");
            }
                Console.SetCursorPosition(2, 19);
                Console.Write("Val: ");
            Console.SetCursorPosition(0, 21);
            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int productId))
            {
                if (!LoginService.IsLoggedIn)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Du måste vara inloggad för att lägga produkter i varukorgen.");
                    Console.ResetColor();
                    Console.ReadKey();
                    return "";
                }

                

                var product = db.Products.FirstOrDefault(p => p.Id == productId && p.IsOnSale);
                if (product == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Produkten hittades inte eller är inte på kampanj.");
                    Console.ResetColor();
                    Console.ReadKey();
                    return "";
                }

                if (product.Stock <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Produkten är slut i lager.");
                    Console.ResetColor();
                    Console.ReadKey();
                    return "";
                }

                CartService.AddToCart(product);

                product.Stock -= 1;
                db.SaveChanges();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{product.Name} har lagts i varukorgen!");
                Console.ResetColor();
                Console.ReadKey();

                return "";
            }


            return input;
            }


        }
    }

