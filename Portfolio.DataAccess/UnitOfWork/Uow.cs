using Portfolio.DataAccess.Context;
using Portfolio.DataAccess.Intrerface;
using Portfolio.DataAccess.Repository;

namespace Portfolio.DataAccess.UnitOfWork;

public class Uow : IUow
{
    private readonly ApplicationContext _context;

    public Uow(ApplicationContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        return new GenericRepository<T>(_context);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
