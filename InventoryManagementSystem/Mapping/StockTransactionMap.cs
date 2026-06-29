using FluentNHibernate.Mapping;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Mapping
{
    public class StockTransactionMap : ClassMap<StockTransaction>
    {
        public StockTransactionMap()
        {
            Table("StockTransactions");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Quantity).Not.Nullable();
            Map(x => x.Type).Not.Nullable().Length(20);
            Map(x => x.TransactionDate).Not.Nullable();
            Map(x => x.Remarks).Nullable();

            References(x => x.Product).Column("ProductId").Not.Nullable();
        }
    }
}