namespace Portfolio.DataAccess.Intrerface;

public interface IUow
{
    IGenericRepository<T> GetRepository<T>() where T : class;
    Task SaveChangesAsync();
}
