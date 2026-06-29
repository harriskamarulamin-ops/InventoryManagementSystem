using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Interfaces;
using NHibernate;

namespace InventoryManagementSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly NHibernate.ISession _session;

        public AccountService(NHibernate.ISession session)
        {
            _session = session;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            // Lookup user by Email
            var user = _session.Query<User>()
                .FirstOrDefault(u => u.Email == email);

            // Verify password using BCrypt
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }
    }
}
