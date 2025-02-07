using Ninject.Modules;
using SankoHospital.Business.Abstract;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.Business.Security;
using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.Core.Helpers;
using SankoHospital.Core.Security.PBKDF2;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.DataAccess.Concrete.NHibernate;
using SankoHospital.DataAccess.Concrete.NHibernate.Helpers;

namespace SankoHospital.Business.DependencyResolvers.Ninject;

public class BusinessModule : NinjectModule
{
    public override void Load()
    {
        //Services-Managers
        Bind<IPatientService>().To<PatientManager>();
        Bind<IRoomService>().To<RoomManager>();
        Bind<IUserService>().To<UserManager>();
        Bind<IPasswordHasher>().To<PasswordHasher>();
        Bind<ITokenService>().To<JwtTokenService>();
        
        //Data Access Layers
        Bind<IPatientDal>().To<NhPatientDal>();
        Bind<IRoomDal>().To<NhRoomDal>();
        Bind<IUserDal>().To<NhUserDal>();
        
        //Helpers
        Bind<NHibernateHelper>().To<SqlServerHelper>();
    }
}