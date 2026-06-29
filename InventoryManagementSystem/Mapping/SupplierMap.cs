using FluentNHibernate.Mapping;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Mapping
{
    public class SupplierMap : ClassMap<Supplier>
    {
        public SupplierMap()
        {
            Table("Suppliers");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name).Not.Nullable().Length(200);
            Map(x => x.ContactAddress).Length(500);
            Map(x => x.PhoneNumber).Length(20);
            Map(x => x.CreatedAt).Not.Nullable();
            HasMany(x => x.Products)
                .Inverse()
                .Cascade.All()
                .KeyColumn("SupplierId");
        }
    }
}
