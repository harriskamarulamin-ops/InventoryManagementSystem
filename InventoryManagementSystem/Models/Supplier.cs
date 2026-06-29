using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class Supplier
    {
        public virtual int Id { get; set; }
        [Required]
        public virtual string Name { get; set; }
        public virtual string ContactAddress { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual IList<Product> Products { get; set; }
        public Supplier()
        {
            Products = new List<Product>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
