using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Concrete.NHibernate.Mappings;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate.Helpers;

public class SqlServerHelper : NHibernateHelper
{
    protected override ISessionFactory InitiliazeFactory()
    {
        return Fluently.Configure()
            .Database(MySQLConfiguration.Standard.ConnectionString(
                c => c.Server("localhost")
                    .Database("SANKO")
                    .Username("root")
                    .Password("12345678")))
            .Mappings(m => m.FluentMappings.
                AddFromAssemblyOf<PatientMap>().AddFromAssemblyOf<RoomMap>().AddFromAssemblyOf<UserMap>())
            .BuildSessionFactory();
    }
}