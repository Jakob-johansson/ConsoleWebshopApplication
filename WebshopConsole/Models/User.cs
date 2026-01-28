using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;

namespace WebshopConsole.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }


        //Dessa två är för att koppla ihop Users med Customer, men samtidigt göra så admin inte behöver vara customer tex, därav "?".
        public int? CustomerId { get; set; }   // nullable
        public Customer? Customer { get; set; }
    }


}
