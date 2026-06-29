using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Interfaces;
using NHibernate;
using NHibernate.Criterion;

namespace InventoryManagementSystem.Services
{
    public class ProductsService : IProducts
    {
        private readonly NHibernate.ISession _session;

        public ProductsService(NHibernate.ISession session)
        {
            _session = session;
        }

        public async Task<IList<Product>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _session.QueryOver<Product>()
                   
                    .ListAsync();
            }

            if (int.TryParse(searchTerm, out int id))
            {
                var product = await _session.GetAsync<Product>(id);
                if (product != null)
                {

                    return new List<Product> { product };
                }
                return new List<Product>();
            }

            Category categoryAlias = null;
            Supplier supplierAlias = null;

            return await _session.QueryOver<Product>()
                .Left.JoinAlias(p => p.Category, () => categoryAlias)
                .Left.JoinAlias(p => p.Supplier, () => supplierAlias)
                .Where(p => p.Name.Contains(searchTerm)||
                p.Variant.Contains(searchTerm)||
                categoryAlias.Name.Contains(searchTerm)||
                supplierAlias.Name.Contains(searchTerm))
                .ListAsync();
        }
    }
}
