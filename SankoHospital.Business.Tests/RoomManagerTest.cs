using Moq;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Tests;

[TestFixture]
public class RoomManagerTest
{
    [Test]
    public void RoomManager_AddUser_Test()
    {
        Mock<IRoomDal> mock = new Mock<IRoomDal>();
        
        RoomManager roomManager = new RoomManager(mock.Object);
        
        roomManager.Add(new Room());
    }
}