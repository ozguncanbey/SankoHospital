using Moq;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Tests;

[TestFixture]
public class RoomOccupancyManagerTest
{
    [Test]
    public void RoomOccupancyManager_AddPatient_Test()
    {
        Mock<IRoomOccupancyDal> mockDal = new Mock<IRoomOccupancyDal>();
        
        RoomOccupancyManager roomOccupancyManager = new RoomOccupancyManager(mockDal.Object);
        
        roomOccupancyManager.Add(new RoomOccupancy());
    }
}