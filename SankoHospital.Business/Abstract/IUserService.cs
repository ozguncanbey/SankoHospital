using SankoHospital.Business.DTOs;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Abstract;

public interface IUserService
{
    List<User> GetAll(); // Tüm kullanıcıları getir
    User GetById(int id); // ID'ye göre kullanıcı getir
    void Add(User user); // Kullanıcı ekle
    void Update(User user); // Kullanıcı güncelle
    void Delete(User user); // Kullanıcı sil
    String Authenticate(string username, string password); // Kullanıcı giriş işlemi
    RoleCountsDto GetAllRoleCounts();
}