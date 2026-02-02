using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;

namespace WebshopConsole.Models
{
    internal class AdminTools
    {


       
        
            public static void ShowAdminMenu()
            {
           

                using var db = new WebshopContext();
            Console.Clear();

            DrawService.DrawBox(20, 1, 40, 5, "  ADMINPANEL  ");
            Console.SetCursorPosition(30, 4);
            Console.Write("Inloggad som ADMIN");

            DrawService.DrawBox(15, 7, 50, 12, "  ADMIN MENY  ");

            Console.SetCursorPosition(20, 9);
            Console.Write("1. Produktöversikt");

            Console.SetCursorPosition(20, 10);
            Console.Write("2. Hantera produkter");

            Console.SetCursorPosition(20, 11);
            Console.Write("3. Orderhantering");
            Console.SetCursorPosition(20, 12);
            Console.Write("4. Kategorier");

            Console.SetCursorPosition(20, 13);
            Console.Write("0. Logga ut");

            Console.SetCursorPosition(20, 16);
            Console.Write("Val: ");

            var choice = Console.ReadLine();


            switch (choice)
                {
                    case "1":
                    try
                    {
                        Console.Clear();

                        var results = db.Products
                            .Include(p => p.Category)
                            .ToList();

                        if (results.Count == 0)
                        {
                            Console.WriteLine("Inga produkter finns ännu.");
                            Console.ReadKey();
                            break;
                        }

                        int boxWidth = 40;
                        int boxHeight = 6;
                        int startX = 2;
                        int startY = 2;
                        int spacingX = 2;
                        int spacingY = 1;

                        int maxColumns = (Console.WindowWidth - 2) / (boxWidth + spacingX);
                        if (maxColumns < 1) maxColumns = 1;

                        int availableHeight = Console.WindowHeight - 6;
                        int maxRows = availableHeight / (boxHeight + spacingY);
                        if (maxRows < 1) maxRows = 1;

                        int boxesPerPage = maxColumns * maxRows;
                        int totalPages = (int)Math.Ceiling((double)results.Count / boxesPerPage);

                        for (int page = 0; page < totalPages; page++)
                        {
                            Console.Clear();
                            Console.WriteLine($"Produkter – Sida {page + 1}/{totalPages}\n");

                            var pageItems = results
                                .Skip(page * boxesPerPage)
                                .Take(boxesPerPage)
                                .ToList();

                            for (int i = 0; i < pageItems.Count; i++)
                            {
                                var p = pageItems[i];

                                int col = i % maxColumns;
                                int row = i / maxColumns;

                                int posX = startX + col * (boxWidth + spacingX);
                                int posY = startY + row * (boxHeight + spacingY);

                                if (posY + boxHeight >= Console.WindowHeight)
                                    continue;

                                DrawService.DrawBox(
                                    posX,
                                    posY,
                                    boxWidth,
                                    boxHeight,
                                    $"{p.Name} ({p.Category?.Name ?? "Ingen kategori"}) [ID: {p.Id}]"
                                );

                                Console.SetCursorPosition(posX + 1, posY + 1);

                                if (p.IsOnSale && p.SalePrice.HasValue)
                                {
                                    Console.Write("Pris: ");
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write($"{p.SalePrice.Value} kr");
                                    Console.ResetColor();
                                    Console.Write($" (Ord. {p.Price} kr)");
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
                            Console.Write("Ange Produkt-ID för detaljer, Enter = nästa sida, Q = tillbaka: ");
                            var input = Console.ReadLine();

                            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                                break;

                            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int productId))
                            {
                                
                                var product = db.Products.FirstOrDefault(p => p.Id == productId);
                                if (product == null)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Produkten hittades inte.");
                                    Console.ResetColor();
                                    Console.ReadKey();
                                    break;
                                }

                                Console.Clear();
                                Console.WriteLine("=== Produktinformation ===\n");

                                Console.WriteLine(product.Name);

                                if (product.IsOnSale && product.SalePrice.HasValue)
                                {
                                    Console.Write("Pris: ");
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write($"{product.SalePrice.Value} kr");
                                    Console.ResetColor();
                                    Console.WriteLine($" (Ord. {product.Price} kr)");
                                }
                                else
                                {
                                    Console.WriteLine($"Pris: {product.Price} kr");
                                }

                                Console.WriteLine($"Färg: {product.Color}");
                                Console.WriteLine($"Storlek: {product.Size}");
                                Console.WriteLine($"I lager: {product.Stock}");

                                Console.WriteLine("\nTryck på valfri tangent för att gå tillbaka...");
                                Console.ReadKey();
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

                    
                    break;
                    case "2":
                        Console.Clear();
                        Console.WriteLine("1. Lägg till produkt! \n" +
                                          "2. Ta bort produkt! \n" );
                                var choice2 = Console.ReadLine();
                        switch (choice2)
                        {
                            case "1":
                       
                            Console.Write("Produktnamn: ");
                            string name = Console.ReadLine();

                            
                            Category category = null;

                            Console.Write("Vill du använda en befintlig kategori? (j/n): ");
                            var useExisting = Console.ReadLine().ToLower();

                            if (useExisting == "j")
                            {
                                var categories = db.Categories.ToList();

                                if (categories.Count == 0)
                                {
                                    Console.WriteLine("Det finns inga kategorier ännu. Skapar ny.");
                                }
                                else
                                {
                                    Console.WriteLine("Befintliga kategorier:");
                                    foreach (var c in categories)
                                    {
                                        Console.WriteLine($"{c.Id}. {c.Name}");
                                    }

                                    Console.Write("Välj kategori-ID: ");
                                    int categoryId = int.Parse(Console.ReadLine());

                                    category = db.Categories.FirstOrDefault(c => c.Id == categoryId);
                                }
                            }

                            //
                            if (category == null)
                            {
                                Console.Write("Ange namn på ny kategori: ");
                                string newCategoryName = Console.ReadLine();

                                category = new Category
                                {
                                    Name = newCategoryName
                                };

                                db.Categories.Add(category);
                                db.SaveChanges();
                            }

                            
                            Console.Write("Pris: ");
                            decimal price = decimal.Parse(Console.ReadLine());

                            Console.Write("Färg: ");
                            string color = Console.ReadLine();

                            Console.Write("Storlek: ");
                            string size = Console.ReadLine();

                            Console.Write("Lagersaldo: ");
                            int stock = int.Parse(Console.ReadLine());

                            Console.Write("Är produkten på kampanj? (j/n): ");
                            bool isOnSale = Console.ReadLine().ToLower() == "j";

                            decimal? salePrice = null;
                            if (isOnSale)
                            {
                                Console.Write("Reapris: ");
                                salePrice = decimal.Parse(Console.ReadLine());
                            }

                            // 
                            db.Products.Add(new Product
                            {
                                Name = name,
                                Category = category,
                                Price = price,
                                Color = color,
                                Size = size,
                                Stock = stock,
                                IsOnSale = isOnSale,
                                SalePrice = salePrice
                            });

                            db.SaveChanges();

                            Console.WriteLine("Produkten har lagts till!");
                            Console.ReadKey();
                            break;
                        case "2":
                      
                            Console.Clear();
                            Console.WriteLine("=== TA BORT PRODUKT ===\n");

                            // Visa alla produkter med ID
                            var products = db.Products.Include(p => p.Category).ToList();

                            if (products.Count == 0)
                            {
                                Console.WriteLine("Det finns inga produkter att ta bort.");
                                Console.ReadKey();
                                break;
                            }

                            foreach (var p in products)
                            {
                                Console.WriteLine($"ID: {p.Id} | {p.Name} | {p.Price} kr");
                            }

                            Console.Write("\nAnge Produkt-ID som ska tas bort: ");
                            if (!int.TryParse(Console.ReadLine(), out int productId))
                            {
                                Console.WriteLine("Ogiltigt ID.");
                                Console.ReadKey();
                                break;
                            }

                            var product = db.Products.FirstOrDefault(p => p.Id == productId);

                            if (product == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Produkten hittades inte.");
                                Console.ResetColor();
                                Console.ReadKey();
                                break;
                            }

                            // Bekräftelse
                            Console.WriteLine($"\nÄr du säker på att du vill ta bort:");
                            Console.WriteLine($"{product.Name} ({product.Price} kr)");
                            Console.Write("Bekräfta (j/n): ");

                            var confirm = Console.ReadLine().ToLower();

                            if (confirm == "j")
                            {
                                db.Products.Remove(product);
                                db.SaveChanges();

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Produkten har tagits bort!");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.WriteLine("Åtgärden avbröts.");
                            }

                            Console.ReadKey();
                            break;

                            break;
                            
                        }

                        break;
                    
                    case "3":
                    OrderService.ShowAllOrders();
                        break;
                case "4":
                case "X":
                    AdminCategoryService.CategoryMenu();
                    break;
                    
                    case "0":
                    LoginService.IsLoggedIn = false;
                    LoginService.AdminOnline = false;
                    LoginService.LoggedInUser = null;
                    break;
                    

                }
            }

        }
    }

