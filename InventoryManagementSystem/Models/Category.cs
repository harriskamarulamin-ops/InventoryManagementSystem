using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;    

namespace InventoryManagementSystem.Models
{
    public class Category
    {
        public virtual int Id { get; set; }
        [Required]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreatedAt { get; set; }

        public virtual IList<Product> Products { get; set; }

        public Category()
        {
            Products = new List<Product>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
