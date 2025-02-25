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
    
    [Test]
    public void Get_all_returns_all_users()
    {
        NhUserDal userDal = new NhUserDal(new SqlServerHelper());
        
        var result = userDal.GetAll();
        
        Assert.That(result.Count(), Is.EqualTo(0));
    }
    
    [Test]
    public void Get_all_returns_all_beds()
    {
        NhBedDal bedDal = new NhBedDal(new SqlServerHelper());
        
        var result = bedDal.GetAll();
        
        Assert.That(result.Count(), Is.EqualTo(25));
    }
    
    [Test]
    public void Get_all_returns_all_patient_daily_record()
    {
        NhPatientDailyRecordDal patientDailyRecordDal = new NhPatientDailyRecordDal(new SqlServerHelper());
        
        var result = patientDailyRecordDal.GetAll();
        
        Assert.That(result.Count(), Is.EqualTo(12));
    }
    
    [Test]
    public void Get_all_returns_all_room_occupancy()
    {
        NhRoomOccupancyDal roomOccupancyDal = new NhRoomOccupancyDal(new SqlServerHelper());
        
        var result = roomOccupancyDal.GetAll();
        
        Assert.That(result.Count(), Is.EqualTo(10));
    }
}