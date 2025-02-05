using SankoHospital.DataAccess.Concrete.NHibernate;
using SankoHospital.DataAccess.Concrete.NHibernate.Helpers;

namespace SankoHospital.DataAccess.Tests.NHibernateTests;

[TestFixture]
public class NHibernateTest
{
    [Test]
    public void Get_all_returns_all_patients()
    {
        NhPatientDal productDal = new NhPatientDal(new SqlServerHelper());
        
        var result = productDal.GetAll();
        
        Assert.That(result.Count(), Is.EqualTo(10));
    }
    
    [Test]
    public void Get_all_returns_all_rooms()
    {
        NhRoomDal roomDal = new NhRoomDal(new SqlServerHelper());
        
        var result = roomDal.GetAll();
        
        Assert.That(result.Count(), Is.EqualTo(10));
    }
}