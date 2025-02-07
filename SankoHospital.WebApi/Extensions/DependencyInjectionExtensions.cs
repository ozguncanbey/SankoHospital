using SankoHospital.Business.Abstract;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.Core.Helpers;
using SankoHospital.Core.Security.PBKDF2;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.DataAccess.Concrete.NHibernate;
using SankoHospital.DataAccess.Concrete.NHibernate.Helpers;

namespace SankoHospital.WebApi.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // Business Layer
            services.AddScoped<IPatientService, PatientManager>();
            services.AddScoped<IRoomService, RoomManager>();
            services.AddScoped<IUserService, UserManager>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            
            // Data Access Layer
            services.AddScoped<IPatientDal, NhPatientDal>();
            services.AddScoped<IRoomDal, NhRoomDal>();
            services.AddScoped<IUserDal, NhUserDal>();

            // NHibernate Helper
            services.AddSingleton<NHibernateHelper, SqlServerHelper>();

            return services;
        }
    }
}