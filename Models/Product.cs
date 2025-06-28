using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StepInStyle.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string ImagePath { get; set; }

        public int? CategoryId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Category Category { get; set; }
    }

}