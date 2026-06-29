using FluentNHibernate.Mapping;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Mapping
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");

            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Username).Unique().Not.Nullable().Length(50);
            Map(x => x.Email).Unique().Not.Nullable().Length(100);
            Map(x => x.PasswordHash).Not.Nullable().Length(255);
            Map(x => x.Role).Not.Nullable().Length(20);
            Map(x => x.CreatedAt).Not.Nullable();
        }
    }
}