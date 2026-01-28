using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;

namespace WebshopConsole.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;

        public List<Product> Products { get; set; } = new();
    }
}
