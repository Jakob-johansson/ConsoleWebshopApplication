using System;
using System.Linq;
using WebshopConsole.Models;

namespace WebshopConsole.Services
{
    internal static class AdminCategoryService
    {
        public static void CategoryMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==================================");
                Console.WriteLine("        ADMIN – KATEGORIER");
                Console.WriteLine("==================================");
                Console.WriteLine("1. Visa alla kategorier");
                Console.WriteLine("2. Lägg till kategori");
                Console.WriteLine("3. Tilldela kategori till produkt");
                Console.WriteLine("0. Tillbaka");
                Console.Write("Välj: ");

                switch (Console.ReadLine())
                {
                    case "1": ShowCategories(); break;
                    case "2": AddCategory(); break;
                    case "3": AssignCategoryToProduct(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Ogiltigt val.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void ShowCategories()
        {
            using var db = new WebshopContext();
            Console.Clear();

            var categories = db.Categories.ToList();

            if (!categories.Any())
            {
                Console.WriteLine("Inga kategorier finns.");
            }
            else
            {
                Console.WriteLine("ID  | NAMN");
                Console.WriteLine("----------------------");

                foreach (var c in categories)
                {
                    Console.WriteLine($"{c.Id,-3} | {c.Name}");
                }
            }

            Console.ReadKey();
        }

        private static void AddCategory()
        {
            using var db = new WebshopContext();
            Console.Clear();

            Console.Write("Ange namn på ny kategori: ");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Namn får inte vara tomt.");
                Console.ReadKey();
                return;
            }

            db.Categories.Add(new Category { Name = name });
            db.SaveChanges();

            Console.WriteLine("Kategori tillagd!");
            Console.ReadKey();
        }

        private static void AssignCategoryToProduct()
        {
            using var db = new WebshopContext();
            Console.Clear();

            var products = db.Products.ToList();
            var categories = db.Categories.ToList();

            if (!products.Any() || !categories.Any())
            {
                Console.WriteLine("Produkter eller kategorier saknas.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("PRODUKTER:");
            Console.WriteLine("----------------------------------");
            foreach (var p in products)
                Console.WriteLine($"{p.Id}. {p.Name}");

            Console.Write("\nAnge produkt-ID: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
                return;

            var product = db.Products.Find(productId);
            if (product == null)
            {
                Console.WriteLine("Produkt hittades inte.");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("KATEGORIER:");
            Console.WriteLine("----------------------------------");
            foreach (var c in categories)
                Console.WriteLine($"{c.Id}. {c.Name}");

            Console.Write("\nAnge kategori-ID: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
                return;

            var category = db.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine("Kategori hittades inte.");
                Console.ReadKey();
                return;
            }

            product.CategoryId = category.Id;
            db.SaveChanges();

            Console.WriteLine($"Produkten '{product.Name}' lades i kategorin '{category.Name}'.");
            Console.ReadKey();
        }
    }
}
