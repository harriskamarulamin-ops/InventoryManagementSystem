

using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class SaleItem
    {
        public virtual int Id { get; set; }
        public virtual Sale Sale { get; set; }
        public virtual Product Product { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity sold must be at least 1.")]
        public virtual int QuantitySold { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
        public virtual decimal UnitPriceAtSale { get; set; }
    }
}
  
