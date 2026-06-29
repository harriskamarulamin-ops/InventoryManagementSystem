using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services.Interfaces
{
    public interface IProducts
    {
        Task<IList<Product>> SearchAsync(string searchTerm);
    }
}
