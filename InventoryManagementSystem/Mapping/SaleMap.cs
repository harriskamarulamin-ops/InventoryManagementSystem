using FluentNHibernate.Mapping;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Mapping
{
    public class SaleMap : ClassMap<Sale>
    {
        public SaleMap()
        {
            Table("Sales");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.SaleDate).Not.Nullable();
            Map(x => x.TotalAmount).Not.Nullable();

            HasMany(x => x.SaleItems)
                .Cascade.AllDeleteOrphan() // If a sale is deleted, clear its children
                .Inverse()
                .KeyColumn("SaleId");
        }
    }
}