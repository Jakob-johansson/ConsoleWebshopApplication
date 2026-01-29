using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;
using System.Security.Cryptography;
using WebshopConsole.Models;
using WebshopConsole.Services;

namespace WebshopConsole
{
    internal class Program
    {
        public static User LoggedInUser = null;
        public static bool loggedInUser = false;

        public static bool AdminOnline = false;

       
        
        static void Main(string[] args)
        {
            //Startmeny

            //Hårdkodar in en admin
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
            bool running = true;
            while (running)
            {
                
                using var db = new WebshopContext();
                var choice = StartPageService.ShowStartPage();
                

                if (!LoginService.IsLoggedIn)
                {
                    switch (choice)
                    {
                        case "1":
                            ShopService.ShowCategoryOverview();
                            break;
                        case "2":
                                LoginService.UserLoginMenu();
                            break;
                        case "3":
                                RegisterService.UserRegisterMenu();
                            break;
                        case "0":
                            
                            break;
                    }
                }
                else
                {
                    switch (choice)
                    {
                        case "1":
                            ShopService.ShowCategoryOverview();
                            break;
                        case "2":
                            CartService.ShowCart();
                            break;
                        case "3":
                            AccountService.ShowAccount();
                            break;
                        case "0":
                            if (LoginService.AdminOnline)
                            {
                                AdminTools.ShowAdminMenu();
                            }
                            break;
                    }
                }
                
            }
            

            //void ShowHeader()
            //{
            //    Console.Clear();
            //    if (LoggedInUser != null)
            //        Console.WriteLine($"Inloggad som: {LoggedInUser.Username}");
            //    else
            //        Console.WriteLine("Ej inloggad");

            //    Console.WriteLine("--------------------------");
            //}
            //if (LoggedInUser != null && LoggedInUser.IsAdmin)
            //{
            //    ShowAdminMenu();
                
            //}
            //else
            //{
               
            //}



            //Skapar admin loggin, endast om det inte finns

           




        }
    }
}
