using Microsoft.EntityFrameworkCore;
using Portfolio.DataAccess.Context;
using Portfolio.DataAccess.Intrerface;
using System.Linq.Expressions;

namespace Portfolio.DataAccess.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationContext _context;

    public GenericRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetByFilterAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false)
    {
        return asNoTracking
            ? await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(filter)
            : await _context.Set<T>().SingleOrDefaultAsync(filter);
    }

    public IQueryable<T> GetQuery()
    {
        return _context.Set<T>().AsQueryable();
    }

    public void Update(T entity, T unchanged)
    {
        _context.Entry(unchanged).CurrentValues.SetValues(entity);
    }
}
