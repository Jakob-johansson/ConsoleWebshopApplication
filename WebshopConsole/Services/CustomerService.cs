using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;

namespace WebshopConsole.Services
{
    internal class CustomerService
    {
        public static Customer GetLoggedInCustomer(WebshopContext db)
        {
            if (LoginService.LoggedInUser == null)
                throw new Exception("Ingen användare är inloggad.");

            var customer = db.Customers
                .FirstOrDefault(c => c.UserId == LoginService.LoggedInUser.Id);

            if (customer == null)
                throw new Exception("Inloggad användare saknar customer-profil.");
            
            return customer;
        }
        
    }
}
