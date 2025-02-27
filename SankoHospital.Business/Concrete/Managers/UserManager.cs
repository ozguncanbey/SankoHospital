using SankoHospital.Business.Abstract;
using SankoHospital.Business.DTOs;
using SankoHospital.Business.Security;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;
using SankoHospital.Core.Security;

namespace SankoHospital.Business.Concrete.Managers
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _jwtTokenService;

        // Dependency Injection
        public UserManager(IUserDal userDal, IPasswordHasher passwordHasher, ITokenService jwtTokenService)
        {
            _userDal = userDal;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public List<User> GetAll()
        {
            return _userDal.GetAll();
        }

        public User GetById(int id)
        {
            return _userDal.GetById(id);
        }

        public void Add(User user)
        {
            // Kullanıcının şifresini hashle
            user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash);
            _userDal.Add(user);
        }

        public void Update(User user)
        {
            _userDal.Update(user);
        }

        public void Delete(User user)
        {
            _userDal.Delete(user);
        }

        public String Authenticate(string username, string password)
        {
            var user = _userDal.GetAll().FirstOrDefault(u => u.Username == username);
            if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
                return null; // Hatalı kullanıcı adı veya şifre

            return _jwtTokenService.GenerateToken(user);
        }

        public RoleCountsDto GetAllRoleCounts()
        {
            var allUsers = _userDal.GetAll();
            // top-level
            var dto = new RoleCountsDto
            {
                TotalUsers = allUsers.Count,
                AdminCount = allUsers.Count(u => u.Role == "Admin"),
                UserCount = allUsers.Count(u => u.Role == "User"),
                ReceptionistCount = allUsers.Count(u => u.Role == "Receptionist"),
                NurseCount = allUsers.Count(u => u.Role == "Nurse"),
                CleanerCount = allUsers.Count(u => u.Role == "Cleaner")
            };
            return dto;
        }

        public List<User> GetFilteredUsers(int? id, string username, string role)
        {
            // Tüm kullanıcıları alalım:
            var users = _userDal.GetAll();

            // ID filtrelemesi
            if (id.HasValue)
            {
                users = users.Where(u => u.Id == id.Value).ToList();
            }

            // Username filtrelemesi (küçük/büyük harf duyarsız)
            if (!string.IsNullOrEmpty(username))
            {
                users = users.Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Role filtrelemesi
            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return users;
        }
    }
}