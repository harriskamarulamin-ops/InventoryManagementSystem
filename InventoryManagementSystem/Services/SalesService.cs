using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Interfaces;
using NHibernate;
using NHibernate.Criterion;

namespace InventoryManagementSystem.Services
{
    public class SalesService : ISales
    {
        private readonly NHibernate.ISession _session;

        public SalesService(NHibernate.ISession session)
        {
            _session = session;
        }

        public async Task<IList<Sale>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                var sales = await _session.QueryOver<Sale>()
                    .OrderBy(s => s.SaleDate).Desc
                    .ListAsync();

                foreach (var sale in sales)
                {
                    NHibernateUtil.Initialize(sale.SaleItems);
                    foreach (var item in sale.SaleItems)
                    {
                        NHibernateUtil.Initialize(item.Product);
                    }
                }
                return sales;
            }

            string cleanSearch = searchTerm.Trim();
            if (cleanSearch.StartsWith("#INV-", StringComparison.OrdinalIgnoreCase))
            {
                cleanSearch = cleanSearch.Substring(5);
            }

            if (int.TryParse(cleanSearch, out int id))
            {
                var sale = await _session.GetAsync<Sale>(id);
                if (sale != null)
                {
                    NHibernateUtil.Initialize(sale.SaleItems);
                    foreach (var item in sale.SaleItems)
                    {
                        NHibernateUtil.Initialize(item.Product);
                    }
                    return new List<Sale> { sale };
                }
            }

            if (DateTime.TryParse(searchTerm, out DateTime searchDate))
            {
                var salesByDate = await _session.QueryOver<Sale>()
                    .Where(s => s.SaleDate >= searchDate.Date && s.SaleDate < searchDate.Date.AddDays(1))
                    .OrderBy(s => s.SaleDate).Desc
                    .ListAsync();

                foreach (var sale in salesByDate)
                {
                    NHibernateUtil.Initialize(sale.SaleItems);
                    foreach (var item in sale.SaleItems)
                    {
                        NHibernateUtil.Initialize(item.Product);
                    }
                }
                return salesByDate;
            }

            SaleItem itemAlias = null;
            Product productAlias = null;

            var results = await _session.QueryOver<Sale>()
                .Left.JoinAlias(s => s.SaleItems, () => itemAlias)
                .Left.JoinAlias(() => itemAlias.Product, () => productAlias)
                .Where(Restrictions.On(() => productAlias.Name).IsInsensitiveLike(searchTerm, MatchMode.Anywhere))
                .OrderBy(s => s.SaleDate).Desc
                .ListAsync();

            var uniqueSales = results.DistinctBy(s => s.Id).ToList();

            foreach (var sale in uniqueSales)
            {
                NHibernateUtil.Initialize(sale.SaleItems);
                foreach (var item in sale.SaleItems)
                {
                    NHibernateUtil.Initialize(item.Product);
                }
            }

            return uniqueSales;
        }
    }
}
