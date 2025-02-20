using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate;

public class NhBedDal : NhEntityRepositoryBase<Bed>, IBedDal
{
    public NhBedDal(NHibernateHelper nHibernateHelper) : base(nHibernateHelper)
    {
    }
}