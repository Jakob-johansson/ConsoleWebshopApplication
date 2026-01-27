using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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

        
        public int CategoryId { get; set; }          // FK
        public Category Category { get; set; } = null!;

        public bool IsOnSale { get; set; }
        //Denna är nullable eftersom den inte behöver vara på rea.
        public decimal? SalePrice { get; set; }

    }
}
