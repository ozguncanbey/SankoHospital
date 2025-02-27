using SankoHospital.Business.Abstract;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.Business.Security;
using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.Core.Security;
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
            services.AddScoped<IBedService, BedManager>();
            services.AddScoped<IPatientDailyRecordService, PatientDailyRecordManager>();
            services.AddScoped<IRoomOccupancyService, RoomOccupancyManager>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, JwtTokenService>();

            // Data Access Layer
            services.AddScoped<IPatientDal, NhPatientDal>();
            services.AddScoped<IRoomDal, NhRoomDal>();
            services.AddScoped<IUserDal, NhUserDal>();
            services.AddScoped<IPatientDailyRecordDal, NhPatientDailyRecordDal>();
            services.AddScoped<IRoomOccupancyDal, NhRoomOccupancyDal>();
            services.AddScoped<IBedDal, NhBedDal>();

            // NHibernate Helper
            services.AddSingleton<NHibernateHelper, SqlServerHelper>();

            return services;
        }
    }
}