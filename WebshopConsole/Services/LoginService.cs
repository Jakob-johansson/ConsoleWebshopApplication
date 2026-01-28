using Azure.Core;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;

namespace WebshopConsole.Services
{
    public class LoginService
    {
        public static bool IsLoggedIn = false;
        public static User LoggedInUser = null;
        public static void UserLoginMenu()
        {
            if (IsLoggedIn == true)
            {
                Console.WriteLine("Du är redan inloggad!");
                Console.Read();
            }
            else
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
                    IsLoggedIn = true;
                    LoggedInUser = user;
                    Console.WriteLine("Admin inloggad");
                    Console.ReadLine();
                    //ShowAdminMenu();
                }
                else
                {
                    IsLoggedIn = true;
                    LoggedInUser = user;
                    Console.WriteLine("Kund inloggad");
                    Console.ReadLine();
                }
            }
        }



    }
}
