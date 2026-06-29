using System;
using System.ComponentModel.DataAnnotations;
using InventoryManagementSystem.Services.Constants;
namespace InventoryManagementSystem.Models
{
    public class StockTransaction
    {
        public virtual int Id { get; set; }
        public virtual Product Product { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public virtual int Quantity { get; set; }

        [Required(ErrorMessage = "Transaction type is required.")]
        
        public virtual string Type { get; set; } // "IN" for stock in, "OUT" for stock out

        public virtual DateTime TransactionDate { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public virtual string Remarks { get; set; }

        public StockTransaction()
        {
            TransactionDate = DateTime.UtcNow;
        }
    }
}
