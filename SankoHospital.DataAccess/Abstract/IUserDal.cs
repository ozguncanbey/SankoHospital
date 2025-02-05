using SankoHospital.Core.DataAccess;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.DataAccess.Abstract;

public interface IUserDal : IEntityRepository<User>
{
    
}