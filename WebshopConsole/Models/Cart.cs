using System;
using System.Collections.Generic;
using System.Text;

namespace WebshopConsole.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<cartItem> Items { get; set; } = new();
    }
}
