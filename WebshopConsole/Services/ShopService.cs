using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;

namespace WebshopConsole.Services
{
    internal class ShopService
    {
        

        public static async Task ShowShop()
        {
            bool inShop = true;

            while (inShop)
            {
                Console.Clear();
                Console.WriteLine("=========== Shoppen ===========");
                Console.WriteLine("1. Visa kategorier");
                Console.WriteLine("2. Sök produkt");
                Console.WriteLine("3. Tillbaka");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ShowCategoryOverview();
                        break;

                    case "2":
                         SearchProductAsync();
                        break;

                    case "3":
                        inShop = false; // ← VIKTIG
                        break;
                }
            }
        }
        public static void SearchProductAsync()
        {
            Console.Clear();
            try
            {
                using var db = new WebshopContext();
                Console.WriteLine("Sök efter produkt eller kategori: ");
                string search = Console.ReadLine()?.ToLower();

                if (string.IsNullOrWhiteSpace(search))
                    throw new ArgumentException("Sökfältet får inte vara tomt.");

                var stopwatch = Stopwatch.StartNew();
                var results = db.Products
                    .Include(p => p.Category)
                    .Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Category.Name.ToLower().Contains(search));
                    
                stopwatch.Stop();

                Console.WriteLine($"Sökningen tog {stopwatch.Elapsed.TotalSeconds} sekunder\n");
                if (!results.Any())
                {
                    Console.WriteLine("Inga produkter hittades.");
                    Console.ReadKey();
                    return;
                }
                foreach (var p in results)
                {
                    Console.WriteLine($"{p.Name} ({p.Category.Name}) Produkt ID: {p.Id}.");

                    if (p.IsOnSale && p.SalePrice.HasValue)
                    {

                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"~~{p.Price} kr~~ ");

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{p.SalePrice} kr");

                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"{p.Price} kr");
                    }

                    Console.WriteLine("----------------------");
                }

                Console.WriteLine("För mer info ange produkt ID: ");

                int productId = int.Parse(Console.ReadLine());

                ShowProductDetails(productId);

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Fel: {ex.Message}");
                Console.ResetColor();
               
            }
        }


        //Tar fram en snygg layout som visar 3 kategorier och 2 produkter från varje kategori, 
        public static async Task ShowCategoryOverview()
        {
            Console.Clear();
            using var db = new WebshopContext();

            //Hämtar produkter och kategorin.
            var products = db.Products
                .Include(p => p.Category)
                .ToList();

            
            var categoryNames = new[] { "herrkläder", "damkläder", "barnkläder" };

            int boxWidth = 30;
            int boxHeight = 8;
            int startY = 2;

            for (int i = 0; i < categoryNames.Length; i++)
            {
                string categoryName = categoryNames[i];

                var categoryProducts = products
                    .Where(p => p.Category.Name.ToLower() == categoryName)
                    .Take(2)
                    .ToList();

                int startX = i * (boxWidth + 2);

                //Drawbox
                DrawService.DrawBox(startX, startY, boxWidth, boxHeight, $" {categoryName.ToUpper()} ");

                int contentY = startY + 2;

                if (!categoryProducts.Any())
                {
                    Console.SetCursorPosition(startX + 2, contentY);
                    Console.Write("Inga produkter");
                    continue;
                }

                foreach (var p in categoryProducts)
                {
                    Console.SetCursorPosition(startX + 2, contentY);
                    Console.Write(p.Name);

                    Console.SetCursorPosition(startX + 2, contentY + 1);

                    if (p.IsOnSale && p.SalePrice.HasValue)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{p.SalePrice} kr ");
                        Console.ResetColor();
                        Console.Write($"({p.Price} kr)");
                    }
                    else
                    {
                        Console.Write($"{p.Price} kr");
                    }

                    contentY += 3;
                }
            }

            Console.SetCursorPosition(0, startY + boxHeight + 2);
            Console.WriteLine("Välj ett av alternativen: \n" +
                              "1. Sök efter kategori eller produkt: \n" +
                              "2. Gå tillbaka till start.");
            
            string categoryChoice = Console.ReadLine();

            switch (categoryChoice)
            {
                case "1":
                     SearchProductAsync();
                    break;
                    
            }

            
        }

        static void ShowProductDetails(int productId)
        {
            try
            {
                using var db = new WebshopContext();

                var product =  db.Products
                    .FirstOrDefault(p => p.Id == productId);
                if (product == null)
                    throw new Exception("Produkten hittades inte.");

                Console.Clear();
                Console.WriteLine(product.Name);
                Console.WriteLine($"Pris: {product.Price} kr");
                Console.WriteLine($"Färg: {product.Color}");
                Console.WriteLine($"Storlek: {product.Size}");
                Console.WriteLine($"I lager: {product.Stock}");

                Console.WriteLine();
                if (LoginService.IsLoggedIn == false)
                {
                    Console.WriteLine("1. Logga in eller registrera dig för att lägga till produkten i kundvagnen");
                    Console.WriteLine("0. Tillbaka");
                    var choice1 = Console.ReadLine();
                    if (choice1 == "1")
                    {
                        Console.Clear();
                        Console.WriteLine("1. Logga in \n" +
                                          "2. Registrera dig");
                        var choice2 = Console.ReadLine();
                        switch (choice2)
                        {
                            case "1":
                                LoginService.UserLoginMenu();
                                break;
                            case "2":
                                RegisterService.UserRegisterMenu();
                                break;
                        }
                    }
                }
                else
                {

                    Console.WriteLine("1. Lägg i varukorg");
                    Console.WriteLine("0. Tillbaka");

                    var choice = Console.ReadLine();

                    if (choice == "1")
                    {
                        CartService.AddToCart(product);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
               
            }
            Console.ReadKey();
        }
    }
}
       