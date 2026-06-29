using System.Threading.Tasks;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User?> AuthenticateAsync(string email, string password);
    }
}
