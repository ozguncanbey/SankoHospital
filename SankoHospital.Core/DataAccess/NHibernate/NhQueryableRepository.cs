using SankoHospital.Core.Entities;

namespace SankoHospital.Core.DataAccess.NHibernate;

public class NhQueryableRepository<T> : IQueryableRepository<T> where T : class, IEntity, new()
{
    private NHibernateHelper _nHibernateHelper;
    private IQueryable<T> _entities;

    public NhQueryableRepository(NHibernateHelper nHibernateHelper)
    {
        _nHibernateHelper = nHibernateHelper;
    }

    public IQueryable<T> Table => Entities;
    
    public virtual IQueryable<T> Entities => _entities == null ? _entities = _nHibernateHelper.OpenSession().Query<T>() : _entities;
}