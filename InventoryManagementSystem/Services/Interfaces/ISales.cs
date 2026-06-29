using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services.Interfaces
{
    public interface ISales
    {
        Task<IList<Sale>> SearchAsync(string searchTerm);
    }
}
