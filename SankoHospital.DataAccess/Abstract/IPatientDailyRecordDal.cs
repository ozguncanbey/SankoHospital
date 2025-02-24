using SankoHospital.Core.DataAccess;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Abstract;

public interface IPatientDailyRecordDal : IEntityRepository<PatientDailyRecord>
{
    
}