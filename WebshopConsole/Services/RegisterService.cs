using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;

namespace WebshopConsole.Services
{
    public class RegisterService
    {
        public static void UserRegisterMenu()
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
        }

    }


