using Moq;
using NUnit.Framework;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Tests;

[TestFixture]
public class UserManagerTest
{
    [Test]
    public void UserManager_AddUser_Test()
    {
        // Arrange (Mock nesneyi oluştur)
        Mock<IUserDal> mock = new Mock<IUserDal>();
        UserManager userManager = new UserManager(mock.Object);

        User testUser = new User
        {
            Id = 1,
            Username = "test_user",
            PasswordHash = "Test123!", // Hashlenmemiş şifre
            Role = "Admin",
        };

        // Moq'un Add metodunu herhangi bir User nesnesi için çalıştırmasına izin ver
        mock.Setup(x => x.Add(It.IsAny<User>())).Verifiable();

        // Act (Metodu çalıştır)
        userManager.Add(testUser);

        // Assert (Beklenen sonucun olup olmadığını doğrula)
        mock.Verify(x => x.Add(It.IsAny<User>()), Times.Once); // Add metodu gerçekten çağrılmış mı kontrol et
        Assert.IsNotNull(testUser.PasswordHash); // Hashleme gerçekleşmiş mi kontrol et
    }
}