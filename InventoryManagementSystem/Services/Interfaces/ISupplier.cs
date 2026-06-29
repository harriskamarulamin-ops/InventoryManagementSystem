using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services.Interfaces
{
    public interface ISupplier
    {
        Task<IList<Supplier>> SearchAsync(string searchTerm);
    }
}
