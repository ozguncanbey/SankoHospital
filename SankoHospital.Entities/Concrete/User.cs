using SankoHospital.Core.Entities;

namespace SankoHospital.Entities.Concrete;

public class User : IEntity
{
    public virtual int Id { get; set; } // Kullanıcı ID (Primary Key)
    public virtual string Username { get; set; } // Kullanıcı Adı (Benzersiz)
    public virtual string PasswordHash { get; set; } // Hashlenmiş Şifre
    public virtual string Role { get; set; } // Kullanıcı Rolü (Admin, Nurse, Cleaner)
    public virtual DateTime CreatedAt { get; set; } // Kullanıcı oluşturulma tarihi
}