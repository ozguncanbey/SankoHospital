using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;
using SankoHospital.Core.Helpers;

namespace SankoHospital.Business.Concrete.Managers
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly IPasswordHasher _passwordHasher;

        // Dependency Injection
        public UserManager(IUserDal userDal, IPasswordHasher passwordHasher)
        {
            _userDal = userDal;
            _passwordHasher = passwordHasher;
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
            if (!string.IsNullOrEmpty(user.PasswordHash)) // Eğer şifre değiştiriliyorsa
            {
                user.PasswordHash = _passwordHasher.HashPassword(user.PasswordHash); 
            }
            _userDal.Update(user);
        }

        public void Delete(User user)
        {
            _userDal.Delete(user);
        }

        public User Authenticate(string username, string password)
        {
            var user = _userDal.GetAll().FirstOrDefault(u => u.Username == username);
            if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
                return null; // Hatalı kullanıcı adı veya şifre

            return user;
        }
    }
}