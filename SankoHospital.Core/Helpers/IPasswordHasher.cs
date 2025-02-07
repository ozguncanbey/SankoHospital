namespace SankoHospital.Core.Helpers;

public interface IPasswordHasher
{
    string HashPassword(string password); // Şifreyi hash'ler
    bool VerifyPassword(string password, string hashedPassword); // Şifreyi doğrular
}