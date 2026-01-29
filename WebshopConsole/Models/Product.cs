using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;
namespace WebshopConsole.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }

        [Required]
        public int Stock { get; set; }

        
        public int CategoryId { get; set; }         
        public Category Category { get; set; } = null!;

        public bool IsOnSale { get; set; }
        
        public decimal? SalePrice { get; set; }

    }
}
