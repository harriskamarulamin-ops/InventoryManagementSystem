using FluentNHibernate.Mapping;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Mapping
{
    public class SaleItemMap : ClassMap<SaleItem>
    {
        public SaleItemMap()
        {
            Table("SaleItems");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.QuantitySold).Not.Nullable();
            Map(x => x.UnitPriceAtSale).Not.Nullable();

            References(x => x.Sale).Column("SaleId").Not.Nullable();
            References(x => x.Product).Column("ProductId").Not.Nullable();
        }
    }
}