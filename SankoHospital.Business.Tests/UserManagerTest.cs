using Moq;
using NUnit.Framework;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.Core.Entities;
using SankoHospital.Core.Helpers;
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
        Mock<IUserDal> mockUserDal = new Mock<IUserDal>();
        Mock<IPasswordHasher> mockPasswordHasher = new Mock<IPasswordHasher>();
        UserManager userManager = new UserManager(mockUserDal.Object, mockPasswordHasher.Object);

        User testUser = new User
        {
            Id = 1,
            Username = "test_user",
            PasswordHash = "Test123!", // Hashlenmemiş şifre
            Role = "Admin"
        };

        string hashedPassword = "hashedTest123!";
        mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<string>())).Returns(hashedPassword);

        // Moq'un Add metodunu herhangi bir User nesnesi için çalıştırmasına izin ver
        mockUserDal.Setup(x => x.Add(It.IsAny<User>())).Verifiable();

        // Act (Metodu çalıştır)
        userManager.Add(testUser);

        // Assert (Beklenen sonucun olup olmadığını doğrula)
        mockUserDal.Verify(x => x.Add(It.IsAny<User>()), Times.Once); // Add metodu gerçekten çağrılmış mı kontrol et
        Assert.AreEqual(hashedPassword, testUser.PasswordHash); // Şifrenin doğru şekilde hash'lendiğini kontrol et
        mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Once); // HashPassword metodunun çağrıldığını kontrol et
    }
}