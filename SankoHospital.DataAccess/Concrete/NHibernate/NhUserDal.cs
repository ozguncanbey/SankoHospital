using SankoHospital.Core.DataAccess.NHibernate;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Concrete.NHibernate;

public class NhUserDal : NhEntityRepositoryBase<User>, IUserDal
{
    public NhUserDal(NHibernateHelper nHibernateHelper) : base(nHibernateHelper)
    {
    }
}