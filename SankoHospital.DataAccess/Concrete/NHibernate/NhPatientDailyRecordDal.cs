using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate;

public class NhPatientDailyRecordDal : NhEntityRepositoryBase<PatientDailyRecord>, IPatientDailyRecordDal
{
    public NhPatientDailyRecordDal(NHibernateHelper nHibernateHelper) : base(nHibernateHelper)
    {
    }
}