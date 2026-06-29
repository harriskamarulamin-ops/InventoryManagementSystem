using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Mapping;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Mapping
{
    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Table("Categories");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name).Not.Nullable().Length(100);
            Map(x => x.Description).Not.Nullable().Length(500);
            Map(x => x.CreatedAt).Not.Nullable();
            HasMany(x => x.Products)
            .Inverse()
            .Cascade.All()
            .KeyColumn("CategoryId");
        }
    }
}
