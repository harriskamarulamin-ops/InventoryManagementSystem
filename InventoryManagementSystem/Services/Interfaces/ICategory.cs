using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services.Interfaces
{
    public interface ICategory
    {
        Task<IList<Category>> SearchAsync(string searchTerm);
    }
}
