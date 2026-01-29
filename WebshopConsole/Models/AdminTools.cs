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
                Console.WriteLine("Välkommen Admin");
                Console.WriteLine("1. Produkt Översikt \n" +
                                  "2. Hantera Produkter \n" +
                                  "3. Köphistorik / Orderstatus \n" +
                                  "4.");
                var key = Console.ReadKey();

                switch (key.KeyChar)
                {
                    case '1':
                        var products = db.Products.ToList();

                        foreach (var p in products)
                        {
                            Console.WriteLine($"{p.Id}. {p.Name} \n" +
                                              $" - {p.Category} \n" +
                                              $"- {p.Price} kr \n" +
                                              $"- {p.Color} \n" +
                                              $"- {p.Size} \n" +
                                              $"(Antal: {p.Stock})");
                        }
                        break;
                    case '2':
                        Console.Clear();
                        Console.WriteLine("1. Lägg till produkt! \n" +
                                          "2. Ta bort produkt! \n" +
                                          "3. Uppdatera befintlig produkt!");
                        var key1 = Console.ReadKey();
                        switch (key1.KeyChar)
                        {
                            case '1': 
                                Console.Write("Produktnamn: ");
                                string name = Console.ReadLine();
                                Console.Write("Kategori: ");
                                string categoryName = Console.ReadLine();
                                Console.Write("Pris: ");
                                decimal price = decimal.Parse(Console.ReadLine());
                                Console.Write("Färg: ");
                                string color = Console.ReadLine();
                                Console.Write("Storlek: ");
                                string size = Console.ReadLine();
                                Console.Write("Lagersaldo: ");
                                int stock = int.Parse(Console.ReadLine());
                                Console.WriteLine("Är produkten på kampanj? (j/n): ");
                                bool isOnSale = Console.ReadLine().ToLower() == "j";

                                decimal? salePrice = null;
                                if (isOnSale)
                                {
                                    Console.Write("Reapris: ");
                                    salePrice = decimal.Parse(Console.ReadLine());
                                }


                                var category = db.Categories
                                 .FirstOrDefault(c => c.Name.ToLower() == categoryName.ToLower());

                                if (category == null)
                                {
                                    category = new Category
                                    {
                                        Name = categoryName
                                    };

                                    db.Categories.Add(category);
                                    db.SaveChanges();
                                }
                                db.Products.Add(new Product
                                {
                                    Name = name,
                                    Category = category,
                                    Price = price,
                                    Color = color,
                                    Size = size,
                                    Stock = stock,
                                    SalePrice = salePrice,
                                    IsOnSale = isOnSale
                                });
                                db.SaveChanges();
                                Console.WriteLine("Produkten tillagd!");
                                Console.ReadLine();
                                break;
                            case '2':
                                Console.Write("Ange produkt-ID att ta bort: ");
                                int id = int.Parse(Console.ReadLine());

                                var product = db.Products.Find(id);

                                if (product != null)
                                {
                                    db.Products.Remove(product);
                                    db.SaveChanges();
                                    Console.WriteLine("Produkten borttagen");
                                }
                                else
                                {
                                    Console.WriteLine("Produkten hittades inte");
                                }
                                break;
                        }

                        break;
                    case '3':
                    OrderService.ShowAllOrders();
                        break;
                    case '4':
                        break;

                }
            }

        }
    }

