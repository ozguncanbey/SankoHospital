using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate;

public class NhPatientDailyRecordDalDal : NhEntityRepositoryBase<PatientDailyRecord>, IPatientDailyRecordDal
{
    public NhPatientDailyRecordDalDal(NHibernateHelper nHibernateHelper) : base(nHibernateHelper)
    {
    }
}