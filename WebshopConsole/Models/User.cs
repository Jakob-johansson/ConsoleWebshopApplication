using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WebshopConsole.Models
{
    
        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }  // plaintext JUST NU
            public bool IsAdmin { get; set; }
        }

    
}
