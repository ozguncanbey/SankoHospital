using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate;

public class NhPatientDal : NhEntityRepositoryBase<Patient>, IPatientDal
{
    public NhPatientDal(NHibernateHelper nHibernateHelper) : base(nHibernateHelper)
    {
    }
}