using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;

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



                
                DrawService.DrawBox(10, 0, 30, 5, "  Jakobs Klädwebshop  ");

               
                DrawService.DrawBox(0, 5, 70, 6, "  Produkter  ");

                int colWidth = 20;
                int startX = 5;
                int startY = 7;

                int col = 2;

                foreach (var p in campaignProducts)
                {
                    Console.SetCursorPosition(col, 6);
                    Console.Write(p.Name);

                    Console.SetCursorPosition(col, 7);
                    Console.Write($"Färg: {p.Color}");

                    Console.SetCursorPosition(col, 8);
                    Console.Write($"Strl: {p.Size}");

                    Console.SetCursorPosition(col, 9);
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write($"{p.SalePrice} kr");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($" ~{p.Price}~");
                    Console.ResetColor();

                    col += 20;
                }

               
                DrawService.DrawBox(0, 17, 90, 9);

                Console.SetCursorPosition(2, 18);
                Console.Write("Välkommen!");

                Console.SetCursorPosition(2, 19);
                Console.Write("Välj ett alternativ nedan:");

                Console.SetCursorPosition(2, 21);
                Console.Write("1. Gå till webshoppen");
            if(LoginService.LoggedInUser == null)
            {
                Console.SetCursorPosition(2, 22);
                Console.Write("2. Logga in");

                Console.SetCursorPosition(2, 23);
                Console.Write("3. Registrera");
            }
            else
            {
                Console.SetCursorPosition(2, 22);
                Console.WriteLine("2. Kundkorg");

                Console.SetCursorPosition(2, 23);
                Console.Write("3. Konto");
            }
                

                Console.SetCursorPosition(2, 24);
                Console.Write("4. Avsluta");

                Console.SetCursorPosition(2, 26);
                Console.Write("Val: ");



                return Console.ReadLine();
            }


        }
    }

