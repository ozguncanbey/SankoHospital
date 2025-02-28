using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate;

public class NhBedOccupancyDal : NhEntityRepositoryBase<BedOccupancy>, IBedOccupancyDal
{
    public NhBedOccupancyDal(NHibernateHelper nHibernateHelper) : base(nHibernateHelper)
    {
    }
}