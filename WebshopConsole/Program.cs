using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using WebshopConsole.Models;

namespace WebshopConsole
{
    internal class Program
    {
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
            }
            //Registrering

        registreringstart:
            Console.Clear();
            if (choice == "2")
            {
                Console.Write("Välj användarnamn: ");
                var username = Console.ReadLine();

                Console.Write("Välj lösenord: ");
                var password = Console.ReadLine();

                using var db = new WebshopContext();

                // Kolla om användarnamnet redan finns
                if (db.Users.Any(u => u.Username == username))
                {
                    Console.WriteLine("Användarnamnet finns redan. \n" +
                                      "Vänligen välj ett annat!");
                    Console.Read();
                    goto registreringstart;
                }

                db.Users.Add(new User
                {
                    Username = username,
                    Password = password,
                    IsAdmin = false
                });

                db.SaveChanges();

                Console.WriteLine("Registrering klar! Du kan nu logga in.");
                goto startmeny;
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

            //Login för användare



        }
    }
}
