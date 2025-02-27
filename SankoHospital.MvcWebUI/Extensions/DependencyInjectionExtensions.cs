using SankoHospital.Business.Abstract;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.Business.Security;
using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.Core.Security;
using SankoHospital.Core.Security.PBKDF2;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.DataAccess.Concrete.NHibernate;
using SankoHospital.DataAccess.Concrete.NHibernate.Helpers;

namespace SankoHospital.MvcWebUI.Extensions
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
            services.AddScoped<IBedOccupancyService, BedOccupancyManager>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, JwtTokenService>();

            // Data Access Layer
            services.AddScoped<IPatientDal, NhPatientDal>();
            services.AddScoped<IRoomDal, NhRoomDal>();
            services.AddScoped<IUserDal, NhUserDal>();
            services.AddScoped<IBedDal, NhBedDal>();
            services.AddScoped<IPatientDailyRecordDal, NhPatientDailyRecordDal>();
            services.AddScoped<IRoomOccupancyDal, NhRoomOccupancyDal>();
            services.AddScoped<IBedOccupancyDal, NhBedOccupancyDal>();
            
            // NHibernate Helper
            services.AddSingleton<NHibernateHelper, SqlServerHelper>();

            // MVC Projesine özel diğer servis kayıtlarını da buraya ekleyebilirsiniz.

            return services;
        }
    }
}