using FluentNHibernate.Mapping;
using SankoHospital.Core.Entities;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate.Mappings;

public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Table("Users");
        LazyLoad();
        Id(x => x.Id).Column("id");
        Map(x => x.Username).Column("username").Not.Nullable().Unique();
        Map(x => x.PasswordHash).Column("password_hash").Not.Nullable();
        Map(x => x.Role).Column("role").Not.Nullable();
        Map(x => x.CreatedAt).Column("created_at").Not.Nullable();
    }
}