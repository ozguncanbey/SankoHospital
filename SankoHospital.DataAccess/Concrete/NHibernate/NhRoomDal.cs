using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate;

public class NhRoomDal : NhEntityRepositoryBase<Room>, IRoomDal
{
    public NhRoomDal(NHibernateHelper nHibernateHelper) : base(nHibernateHelper)
    {
    }
}