using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class Product
    {
        public virtual int Id { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(200, ErrorMessage = "Product Name cannot exceed 200 characters.")]
        public virtual string Name { get; set; }

        [Required(ErrorMessage = "Variant is required.")]
        [StringLength(100, ErrorMessage = "Variant cannot exceed 100 characters.")]
        public virtual string Variant { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Cost Price must be greater than zero.")]
        public virtual decimal CostPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Retail Price must be greater than zero.")]
        public virtual decimal RetailPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Current Stock must be non-negative.")]
        public virtual int CurrentStock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Min Stock Threshold must be non-negative.")]
        public virtual int MinStockThreshold { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual Category Category { get; set; }

        public virtual Supplier Supplier { get; set; }  

        public Product()
        {
            CurrentStock = 0;
            MinStockThreshold = 0;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
