using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using System.Security.Cryptography;
using WebshopConsole.Models;

namespace WebshopConsole
{
    internal class Program
    {
        static User LoggedInUser = null;

         bool AdminOnline = false;

        static void DrawBox(int left, int top, int width, int height, string title = "")
        {
            // Top
            Console.SetCursorPosition(left, top);
            Console.Write("┌" + new string('─', width - 2) + "┐");

            // Title
            if (!string.IsNullOrEmpty(title))
            {
                Console.SetCursorPosition(left + 2, top);
                Console.Write(title);
            }

            // Sides
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.Write("│");
                Console.SetCursorPosition(left + width - 1, top + i);
                Console.Write("│");
            }

            // Bottom
            Console.SetCursorPosition(left, top + height - 1);
            Console.Write("└" + new string('─', width - 2) + "┘");
        }
            static string ShowStartPage()
            {
                Console.Clear();

                using var db = new WebshopContext();
            var products = db.Products;
            var campaignProducts = db.Products
                .Where(p => p.IsOnSale)
                .Take(3)
                .ToList();
                
                    

                // Header
                DrawBox(10, 0, 30, 5, "  Jakobs Klädwebshop  ");

                // Produktbox
                DrawBox(0, 5, 70, 6, "  Produkter  ");

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

            // Meny
            DrawBox(0, 17, 90, 9);

                Console.SetCursorPosition(2, 18);
                Console.Write("Välkommen!");

                Console.SetCursorPosition(2, 19);
                Console.Write("Välj ett alternativ nedan:");

                Console.SetCursorPosition(2, 21);
                Console.Write("1. Se alla produkter");

                Console.SetCursorPosition(2, 22);
                Console.Write("2. Logga in");

                Console.SetCursorPosition(2, 23);
                Console.Write("3. Registrera");

                Console.SetCursorPosition(2, 24);
                Console.Write("4. Avsluta");

                Console.SetCursorPosition(2, 26);
                Console.Write("Val: ");

            return Console.ReadLine();
            }

        
        static void ShowShopLayout()
        {
           

                DrawBox(0, 0, 30, 5, "# Fina butiken #");


                DrawBox(0, 6, 30, 7, "Erbjudande 1");
                Console.SetCursorPosition(2, 7);
                Console.Write("Tröja");

                Console.SetCursorPosition(2, 8);
                Console.Write("Pris: 149 kr");

                Console.SetCursorPosition(2, 9);
                Console.Write("Tryck 4 för att köpa");

                Console.ReadLine();

        }
        static void UserLoginMenu()
        {
            Console.Clear();
            using var db = new WebshopContext();
            Console.Write("Användarnamn: ");
            var username = Console.ReadLine();

            Console.Write("Lösenord: ");
            var password = Console.ReadLine();
            var user = db.Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            if (user == null)
            {
                Console.WriteLine("Fel inlogg");
                
            }

            if (user.IsAdmin)
            {
                LoggedInUser = user;
                Console.WriteLine("Admin inloggad");
                Console.ReadLine();
                ShowAdminMenu();
            }
            else
            {
                LoggedInUser = user;
                Console.WriteLine("Kund inloggad");
                Console.ReadLine();
            }
            
        }
        static void UserRegisterMenu()
        {
            while (true)
            {
                Console.Write("Användarnamn: ");
                string username = Console.ReadLine();
                Console.Write("Lösenord: ");
                string password = Console.ReadLine();

                Console.WriteLine("Förnamn: ");
                string firstname = Console.ReadLine();
                Console.WriteLine("Efternamn: ");
                string lastname = Console.ReadLine();
                Console.WriteLine("Adress: ");
                string adress = Console.ReadLine();
                Console.WriteLine("Stad: ");
                string city = Console.ReadLine();
                Console.WriteLine("Land: ");
                string country = Console.ReadLine();
                Console.WriteLine("Telefonnummer: ");
                int phonenumber = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Ålder: ");
                int age = Convert.ToInt32(Console.ReadLine());


                using var db = new WebshopContext();
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Användarnamn och lösenord får inte vara tomma.");
                    continue;
                }

                if (db.Users.Any(u => u.Username == username))
                {
                    Console.WriteLine("Användarnamnet finns redan. F" +
                        "örsök igen.");
                    continue;
                }

                var user = new User
                {
                    Username = username,
                    Password = password,
                    IsAdmin = false,
                    Customer = new Customer
                    {
                        FirstName = firstname,
                        LastName = lastname,
                        Address = adress,
                        City = city,
                        Country = country,
                        PhoneNumber = phonenumber,
                        Age = age
                    }
                };
                db.Users.Add(user);
                db.SaveChanges();
                Console.WriteLine("Registrering lyckades!");
                break;
            }
        }
        
        static void ShowAdminMenu()
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
                        case '1': //Lägger till en produkt i databasen.
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
                    break;
                case '4':
                    break;

            }
        }
        static List<Product> GetProducts()
        {
            using var db = new WebshopContext();

            return db.Products
                .Include(p => p.Category)
                .ToList();
        }
        static void Main(string[] args)
        {
            //Startmeny
       
            bool running = true;
            while (running)
            {
                using var db = new WebshopContext();
                string choice = ShowStartPage();
                switch (choice)
                {
                    case "1":
                        UserLoginMenu();
                        break;
                    case "2":
                        UserRegisterMenu();
                        break;
                    case "3":

                        break;
                    case "0":
                        break;
                }
            }
            

            void ShowHeader()
            {
                Console.Clear();
                if (LoggedInUser != null)
                    Console.WriteLine($"Inloggad som: {LoggedInUser.Username}");
                else
                    Console.WriteLine("Ej inloggad");

                Console.WriteLine("--------------------------");
            }
            if (LoggedInUser != null && LoggedInUser.IsAdmin)
            {
                ShowAdminMenu();
                
            }
            else
            {
               
            }



            //Skapar admin loggin, endast om det inte finns

            //Console.WriteLine("====== Välkommen till Jakobs Klädwebshop! ====== ");
            //using (var db = new WebshopContext())
            //{
            //    if (!db.Users.Any())
            //    {
            //        db.Users.Add(new User
            //        {
            //            Username = "admin",
            //            Password = "admin123",
            //            IsAdmin = true,

            //        });
            //        db.SaveChanges();
            //    }
            //}




        }
    }
}
