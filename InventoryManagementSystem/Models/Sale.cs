using System;
using System.Collections.Generic;
namespace InventoryManagementSystem.Models

{
    public class Sale
    {
        public virtual int Id { get; set; }
        public virtual DateTime SaleDate { get; set; }
        public virtual decimal TotalAmount { get; set; }
        public virtual IList<SaleItem> SaleItems { get; set; }
        public Sale()
        {
            SaleItems = new List<SaleItem>();
            SaleDate = DateTime.UtcNow;
        }
    }
}
