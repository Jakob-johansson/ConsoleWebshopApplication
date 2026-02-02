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
                        case "a":
                            ShopService.ShowCategoryOverview();
                            break;
                        case "b":
                                LoginService.UserLoginMenu();
                            break;
                        case "c":
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
                        case "a":
                            ShopService.ShowCategoryOverview();
                            break;
                        case "b":
                            CartService.ShowCart();
                            break;
                        case "c":
                            AccountService.ShowAccount();
                            break;
                        case "d":
                            if (LoginService.AdminOnline)
                            {
                                AdminTools.ShowAdminMenu();
                            }
                            else if(LoginService.IsLoggedIn)
                            {
                                LoginService.IsLoggedIn = false;
                                LoginService.LoggedInUser = null;
                            }
                            break;
                    }
                }
                
            }
        }
    }
}
