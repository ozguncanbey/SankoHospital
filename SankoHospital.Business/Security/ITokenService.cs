using SankoHospital.Entities.Concrete;

namespace SankoHospital.Business.Security;

public interface ITokenService
{
    string GenerateToken(User user);
}