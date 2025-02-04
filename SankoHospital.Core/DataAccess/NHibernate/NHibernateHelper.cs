using NHibernate;

namespace SankoHospital.Core.DataAccess.NHibernate;

public abstract class NHibernateHelper : IDisposable
{
    private static ISessionFactory _sessionFactory;
    
    public virtual ISessionFactory SessionFactory
    {
        get { return _sessionFactory ?? (_sessionFactory = InitiliazeFactory()); }
    }
    
    protected abstract ISessionFactory InitiliazeFactory();

    public virtual ISession OpenSession()
    {
        return SessionFactory.OpenSession();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}