using SankoHospital.Core.Entities;

namespace SankoHospital.Core.DataAccess.NHibernate;

public class NhEntityRepositoryBase<TEntity> : IEntityRepository<TEntity> where TEntity : class, IEntity, new()
{
    private NHibernateHelper _nHibernateHelper;

    public NhEntityRepositoryBase(NHibernateHelper nHibernateHelper)
    {
        _nHibernateHelper = nHibernateHelper;
    }

    public List<TEntity> GetAll()
    {
        using (var session = _nHibernateHelper.OpenSession())
        {
            return session.Query<TEntity>().ToList();
        }
    }

    public TEntity GetById(int id)
    {
        using (var session = _nHibernateHelper.OpenSession())
        {
            return session.Get<TEntity>(id);
        }
    }

    public void Add(TEntity entity)
    {
        using (var session = _nHibernateHelper.OpenSession())
        {
            session.Save(entity);
        }
    }

    public void Update(TEntity entity)
    {
        using (var session = _nHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                session.Update(entity);
                transaction.Commit(); 
            }
        }
    }

    public void Delete(TEntity entity)
    {
        using (var session = _nHibernateHelper.OpenSession())
        {
            using (var transaction = session.BeginTransaction())
            {
                session.Delete(entity);
                transaction.Commit();
            }
        }
    }
}