using System.Security.Cryptography;
using System.Text;
using SankoHospital.Business.Abstract;
using SankoHospital.DataAccess.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Concrete.Managers;

public class UserManager : IUserService
{
    private readonly IUserDal _userDal;

    public UserManager(IUserDal userDal)
    {
        _userDal = userDal;
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
        user.PasswordHash = HashPassword(user.PasswordHash); // Şifreyi hashle
        _userDal.Add(user);
    }

    public void Update(User user)
    {
        if (!string.IsNullOrEmpty(user.PasswordHash)) // Şifre boş değilse
        {
            user.PasswordHash = HashPassword(user.PasswordHash); // Şifreyi hashle
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
        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;
        
        return user;
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }
}