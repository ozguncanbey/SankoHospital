using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Security
{
    public class JwtTokenService : ITokenService
    {
        private const string SecretKey = "BuCokGucluBirGizliAnahtar!123456789"; // güçlü ve uzun bir key kullan!
        private readonly byte[] _key = Encoding.UTF8.GetBytes(SecretKey);

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(_key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Role, user.Role), // Kullanıcı rolü
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "sankohospital.com",
                audience: "sankohospital.com",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),  // 7 gün geçerlilik süresi
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}