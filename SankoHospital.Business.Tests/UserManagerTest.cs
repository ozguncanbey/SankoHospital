using Moq;
using SankoHospital.Business.Concrete.Managers;
using SankoHospital.Core.Helpers;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;
using SankoHospital.Business.Security;

namespace SankoHospital.Business.Tests
{
    [TestFixture]
    public class UserManagerTest
    {
        private Mock<IUserDal> _mockUserDal;
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private Mock<ITokenService> _mockJwtTokenService;
        private UserManager _userManager;

        [SetUp]
        public void Setup()
        {
            _mockUserDal = new Mock<IUserDal>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockJwtTokenService = new Mock<ITokenService>();

            _userManager = new UserManager(_mockUserDal.Object, _mockPasswordHasher.Object, _mockJwtTokenService.Object);
        }

        [Test]
        public void Add_User_Should_HashPassword_And_Save()
        {
            // Arrange
            var testUser = new User
            {
                Id = 1,
                Username = "test_user",
                PasswordHash = "Test123!",
                Role = "Admin"
            };

            string hashedPassword = "100000:abc123:saltedHash"; // Örnek PBKDF2 hash

            _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<string>())).Returns(hashedPassword);
            _mockUserDal.Setup(x => x.Add(It.IsAny<User>())).Verifiable();

            // Act
            _userManager.Add(testUser);

            // Assert
            _mockUserDal.Verify(x => x.Add(It.IsAny<User>()), Times.Once);
            Assert.AreEqual(hashedPassword, testUser.PasswordHash);
            _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Authenticate_ValidUser_Should_Return_JwtToken()
        {
            // Arrange
            var testUser = new User
            {
                Id = 1,
                Username = "test_user",
                PasswordHash = "100000:abc123:saltedHash",
                Role = "Admin"
            };

            _mockUserDal.Setup(x => x.GetAll()).Returns(new List<User> { testUser });
            _mockPasswordHasher.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Gerçek JWT'nin mock edilen değeriyle aynı olduğundan emin ol
            string expectedToken = "valid.jwt.token";
            _mockJwtTokenService.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns(expectedToken);

            // Act
            var token = _userManager.Authenticate("test_user", "Test123!");

            // Debug için konsola yaz
            Console.WriteLine($"Generated Token: {token}");

            // Assert
            Assert.IsNotNull(token, "Token null döndü, JWT üretilemiyor!");

            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockJwtTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Once);
        }


        [Test]
        public void Authenticate_InvalidUser_Should_Return_Null()
        {
            // Arrange
            _mockUserDal.Setup(x => x.GetAll()).Returns(new List<User>());
            _mockPasswordHasher.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var authenticatedUser = _userManager.Authenticate("wrong_user", "WrongPassword");

            // Assert
            Assert.IsNull(authenticatedUser);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockJwtTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }
    }
}
