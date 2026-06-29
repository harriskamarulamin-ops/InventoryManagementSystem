using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Interfaces;
using NHibernate;
using NHibernate.Criterion;

namespace InventoryManagementSystem.Services
{
    public class CategoryService : ICategory
    {
        private readonly NHibernate.ISession _session;

        public CategoryService(NHibernate.ISession session)
        {
            _session = session;
        }

        public async Task<IList<Category>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _session.QueryOver<Category>().ListAsync();
            }

            if (int.TryParse(searchTerm, out int id))
            {
                var category = await _session.GetAsync<Category>(id);
                if (category != null)
                {
                    return new List<Category> { category };
                }
                return new List<Category>();
            }

            return await _session.QueryOver<Category>()
                .Where(Restrictions.Or(
                    Restrictions.On<Category>(c => c.Name).IsInsensitiveLike(searchTerm, MatchMode.Anywhere),
                    Restrictions.On<Category>(c => c.Description).IsInsensitiveLike(searchTerm, MatchMode.Anywhere)
                ))
                .ListAsync();
        }
    }
}
