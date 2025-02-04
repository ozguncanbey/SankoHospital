using Ninject.Modules;
using SankoHospital.Business.Abstract;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.DataAccess.Concrete.NHibernate;
using SankoHospital.DataAccess.Concrete.NHibernate.Helpers;

namespace SankoHospital.Business.DependencyResolvers.Ninject;

public class BusinessModule : NinjectModule
{
    public override void Load()
    {
        Bind<IPatientService>().To<PatientManager>();
        Bind<IPatientDal>().To<NhPatientDal>();
        Bind<NHibernateHelper>().To<SqlServerHelper>();
    }
}