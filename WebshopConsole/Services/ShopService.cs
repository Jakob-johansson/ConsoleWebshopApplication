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
                        inShop = false; 
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
                        p.Category.Name.ToLower().Contains(search))
                    .ToList(); 
                stopwatch.Stop();

               

                if (!results.Any())
                {
                    Console.WriteLine("Inga produkter hittades.");
                    Console.ReadKey();
                    return;
                }

                int boxWidth = 40;
                int boxHeight = 6;
                int startX = 2;
                int startY = 2;
                int spacingX = 2;
                int spacingY = 1;

                int maxColumns = (Console.WindowWidth - 2) / (boxWidth + spacingX);
                if (maxColumns < 1) maxColumns = 1;

                int availableHeight = Console.WindowHeight - 5;
                int maxRows = availableHeight / (boxHeight + spacingY);
                if (maxRows < 1) maxRows = 1;

                int boxesPerPage = maxColumns * maxRows;
                int totalPages = (int)Math.Ceiling((double)results.Count / boxesPerPage);

                for (int page = 0; page < totalPages; page++)
                {
                    Console.Clear();
                    Console.WriteLine($"Sökningen tog {stopwatch.Elapsed.TotalSeconds:F2} sekunder\n");
                    Console.WriteLine($"Resultat sida {page + 1}/{totalPages}");

                    var pageItems = results.Skip(page * boxesPerPage).Take(boxesPerPage).ToList();

                    for (int i = 0; i < pageItems.Count; i++)
                    {
                        var p = pageItems[i];

                        int col = i % maxColumns;
                        int row = i / maxColumns;

                        int posX = startX + col * (boxWidth + spacingX);
                        int posY = startY + row * (boxHeight + spacingY);

                        if (posY + boxHeight >= Console.WindowHeight) continue; 

                        DrawService.DrawBox(posX, posY, boxWidth, boxHeight, $"{p.Name} ({p.Category.Name}) [ID: {p.Id}]");

                      
                        Console.SetCursorPosition(posX + 1, posY + 1);

                       
                        if (p.IsOnSale && p.SalePrice.HasValue)
                        {
                            Console.Write("Pris: ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"{p.SalePrice.Value} kr");
                            Console.ResetColor();
                            Console.Write($" (Ord {p.Price} kr)");
                        }
                        else
                        {
                            Console.Write($"Pris: {p.Price} kr");
                        }

                        Console.SetCursorPosition(posX + 1, posY + 2);
                        Console.Write($"Färg: {p.Color}");
                        Console.SetCursorPosition(posX + 1, posY + 3);
                        Console.Write($"Storlek: {p.Size}");
                        Console.SetCursorPosition(posX + 1, posY + 4);
                        Console.Write($"I lager: {p.Stock}");
                    }

                    Console.SetCursorPosition(0, startY + maxRows * (boxHeight + spacingY) + 1);
                    Console.WriteLine("Ange Produkt ID för detaljer eller tryck Enter för nästa sida:");
                    var input = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int productId))
                    {
                        ShowProductDetails(productId);
                        break; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Fel: {ex.Message}");
                Console.ResetColor();
                Console.ReadKey();
            }
        }




        
        public static async Task ShowCategoryOverview()
        {
            Console.Clear();
            using var db = new WebshopContext();

           
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

       public static void ShowProductDetails(int productId)
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

                    switch (choice)
                    {
                        case "1":
                            CartService.AddToCart(product);
                            Console.WriteLine($"1st {product.Name} lades till i kundvagnen");
                            break;
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
       