using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate;

public class NhRoomOccupancyDal : NhEntityRepositoryBase<RoomOccupancy>, IRoomOccupancyDal
{
    public NhRoomOccupancyDal(NHibernateHelper nHibernateHelper) : base(nHibernateHelper)
    {
    }
}