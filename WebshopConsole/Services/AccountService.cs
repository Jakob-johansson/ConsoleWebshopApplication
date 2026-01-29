using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;

namespace WebshopConsole.Services
{
    internal class AccountService
    {
        public static void ShowAccount()
        {
            Console.Clear();
            using var db = new WebshopContext();
            var customer = CustomerService.GetLoggedInCustomer(db);

            int boxWidth = 50;
            int boxHeight = 12;
            int startX = 10;
            int startY = 2;

            DrawService.DrawBox(startX, startY, boxWidth, boxHeight, "  MITT KONTO  ");

            int y = startY + 2;
            int x = startX + 2;

            WriteField(x, y++, "Namn", $"{customer.FirstName} {customer.LastName}");
            WriteField(x, y++, "Adress", customer.Address);
            WriteField(x, y++, "Stad", customer.City);
            WriteField(x, y++, "Land", customer.Country);
            WriteField(x, y++, "Telefon", customer.PhoneNumber.ToString());
            WriteField(x, y++, "Ålder", customer.Age.ToString());

            Console.SetCursorPosition(startX, startY + boxHeight + 2);
            Console.WriteLine("Välj ett alternativ:");
            Console.WriteLine("1. Uppdatera uppgifter");
            Console.WriteLine("2. Mina ordrar");
            Console.WriteLine("3. Tillbaka");

            var choice = Console.ReadLine();

            if (choice == "1")
                ShowUpdateAccount(customer, db);
            else if(choice == "2")
            {
                OrderService.ShowMyOrders();
            }
        }

        private static void WriteField(int x, int y, string label, string value)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"{label,-12}: {value}");
        }
    
    private static void ShowUpdateAccount(Customer customer, WebshopContext db)
        {
            Console.Clear();

            int boxWidth = 60;
            int boxHeight = 14;
            int startX = 8;
            int startY = 2;

            DrawService.DrawBox(startX, startY, boxWidth, boxHeight, "  UPPDATERA KONTO  ");

            int y = startY + 2;
            int x = startX + 2;

            customer.FirstName = Ask(x, ref y, "Förnamn", customer.FirstName);
            customer.LastName = Ask(x, ref y, "Efternamn", customer.LastName);
            customer.Address = Ask(x, ref y, "Adress", customer.Address);
            customer.City = Ask(x, ref y, "Stad", customer.City);
            customer.Country = Ask(x, ref y, "Land", customer.Country);
            customer.PhoneNumber = AskInt(x, ref y, "Telefon", customer.PhoneNumber);
            customer.Age = AskInt(x, ref y, "Ålder", customer.Age);

            db.SaveChanges();

            Console.SetCursorPosition(startX + 2, startY + boxHeight - 2);
            Console.Write("Uppgifterna har sparats!");
            Console.ReadKey();
        }

        private static string Ask(int x, ref int y, string label, string current)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"{label} ({current}): ");
            var input = Console.ReadLine();
            y++;
            return string.IsNullOrWhiteSpace(input) ? current : input;
        }

        private static int AskInt(int x, ref int y, string label, int current)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"{label} ({current}): ");
            var input = Console.ReadLine();
            y++;
            return int.TryParse(input, out var value) ? value : current;
        }
    }
}

