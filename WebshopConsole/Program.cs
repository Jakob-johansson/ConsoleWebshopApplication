using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using WebshopConsole.Models;

namespace WebshopConsole
{
    internal class Program
    {
        static User LoggedInUser = null;

        static void Main(string[] args)
        {

            //Startmeny
            Console.WriteLine("====== Välkommen till Jakobs Klädwebshop! ====== ");
        startmeny:
            Console.WriteLine("1. Logga in \n " +
                              "2. Registrera dig \n" +
                              "3. Alternativ");

            var choice = Console.ReadLine();
            //Inlogg

            if (choice == "1")
            {
                Console.Write("Username: ");
                var username = Console.ReadLine();

                Console.Write("Password: ");
                var password = Console.ReadLine();

                using var db = new WebshopContext();

                var user = db.Users.FirstOrDefault(u =>
                    u.Username == username && u.Password == password);

                if (user == null)
                {
                    Console.WriteLine("Fel inlogg");
                    return;
                }

                if (user.IsAdmin)
                    Console.WriteLine("Admin inloggad");
                else
                    Console.WriteLine("Kund inloggad");
                LoggedInUser = user;
            }
           
            //Registrering
            Console.Clear();
            if (choice == "2")
            {

                while (true)
                {
                    Console.Write("Användarnamn: ");
                    string username = Console.ReadLine();

                    Console.Write("Lösenord: ");
                    string password = Console.ReadLine();
                    using var db = new WebshopContext();
                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Användarnamn och lösenord får inte vara tomma.");
                        continue;
                    }

                    if (db.Users.Any(u => u.Username == username))
                    {
                        Console.WriteLine("Användarnamnet finns redan. Försök igen.");
                        continue; 
                    }

                    db.Users.Add(new User
                    {
                        Username = username,
                        Password = password,
                        IsAdmin = false
                    });

                    db.SaveChanges();
                    Console.WriteLine("Registrering lyckades!");
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

            //Skapar admin loggin, endast om det inte finns 
            //using (var db = new WebshopContext())
            //{
            //    if (!db.Users.Any())
            //    {
            //        db.Users.Add(new User{
            //            Username = "admin",
            //            Password = "admin123",
            //            IsAdmin = true
            //        });
            //        db.SaveChanges();
            //    }
            //}

        



        }
    }
}
