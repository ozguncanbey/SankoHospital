using SankoHospital.Core.Entities;

namespace SankoHospital.Core.DataAccess;

public interface IQueryableRepository<out T> where T : class, IEntity, new()
{
    IQueryable<T> Table { get; }
}