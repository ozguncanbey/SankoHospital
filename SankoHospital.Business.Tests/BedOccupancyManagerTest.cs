using Moq;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Tests;

[TestFixture]
public class BedOccupancyManagerTest
{
    [Test]
    public void BedOccupancyManager_Add_BedOccupancy_Test()
    {
        Mock<IBedOccupancyDal> mockDal = new Mock<IBedOccupancyDal>();
        
        BedOccupancyManager bedOccupancyManager = new BedOccupancyManager(mockDal.Object);
        
        bedOccupancyManager.Add(new BedOccupancy());
    }
}