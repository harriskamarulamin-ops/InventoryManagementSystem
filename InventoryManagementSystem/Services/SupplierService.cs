using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Interfaces;
using NHibernate;
using NHibernate.Criterion;

namespace InventoryManagementSystem.Services
{
    public class SupplierService : ISupplier
    {
        private readonly NHibernate.ISession _session;

        public SupplierService(NHibernate.ISession session)
        {
            _session = session;
        }

        public async Task<IList<Supplier>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _session.QueryOver<Supplier>().ListAsync();
            }

            if (int.TryParse(searchTerm, out int id))
            {
                var supplier = await _session.GetAsync<Supplier>(id);
                if (supplier != null)
                {
                    return new List<Supplier> { supplier };
                }
                return new List<Supplier>();
            }

            return await _session.QueryOver<Supplier>()
                .Where(Restrictions.Or(
                    Restrictions.Or(
                        Restrictions.On<Supplier>(s => s.Name).IsInsensitiveLike(searchTerm, MatchMode.Anywhere),
                        Restrictions.On<Supplier>(s => s.ContactAddress).IsInsensitiveLike(searchTerm, MatchMode.Anywhere)
                    ),
                    Restrictions.On<Supplier>(s => s.PhoneNumber).IsInsensitiveLike(searchTerm, MatchMode.Anywhere)
                ))
                .ListAsync();
        }
    }
}
