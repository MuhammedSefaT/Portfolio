using System.Linq.Expressions;

namespace Portfolio.DataAccess.Intrerface;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetAsync(int id);
    Task<T?> GetByFilterAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false);
    Task CreateAsync(T entity);
    void Update(T entity, T unchanged);
    void Delete(T entity);
    IQueryable<T> GetQuery();
}
