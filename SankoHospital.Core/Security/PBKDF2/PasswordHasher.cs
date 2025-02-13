using System.Security.Cryptography;

namespace SankoHospital.Core.Security.PBKDF2
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 16 byte salt
        private const int HashSize = 32; // 32 byte hash
        private const int Iterations = 100000; // 100.000 iterasyon (Önerilen minimum)

        // Şifreyi hashle
        public string HashPassword(string password)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt); // Rastgele salt oluştur

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(HashSize);
                    
                    // Format: Iteration:Salt:Hash
                    return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
                }
            }
        }

        // Şifreyi doğrula
        public bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 3) return false; // Geçersiz format

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedPasswordHash = Convert.FromBase64String(parts[2]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                // Hashleri güvenli bir şekilde karşılaştır
                return CryptographicOperations.FixedTimeEquals(hash, storedPasswordHash);
            }
        }
    }
}