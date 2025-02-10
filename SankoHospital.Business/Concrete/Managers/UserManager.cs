using SankoHospital.Business.Abstract;
using SankoHospital.Business.DTOs;
using SankoHospital.Business.Security;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;
using SankoHospital.Core.Helpers;

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
            //Şifre değişikliği
            /*if (!string.IsNullOrEmpty(user.PasswordHash)) // Eğer şifre değiştiriliyorsa
            {
                user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash); 
            }*/
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
    }
}