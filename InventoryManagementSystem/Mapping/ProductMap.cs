using FluentNHibernate.Mapping;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Mapping
{
    public class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Table("Products");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name).Not.Nullable().Length(200);
            Map(x => x.Variant).Not.Nullable().Length(100);
            Map(x => x.CostPrice).Not.Nullable();
            Map(x => x.RetailPrice).Not.Nullable();
            Map(x => x.CurrentStock).Not.Nullable();
            Map(x => x.MinStockThreshold).Not.Nullable();
            Map(x => x.CreatedAt).Not.Nullable();

            References(x => x.Category)
                .Column("CategoryId")
                .Not.Nullable();

            References(x => x.Supplier)
                .Column("SupplierId")
                .Not.Nullable();
       
        }
    }
}
//i stopped here