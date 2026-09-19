namespace Portfolyo.Web.Application.Common.Exceptions;

/// <summary>
/// İstenen kayıt bulunamadığında fırlatılır.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException For<T>(Guid id)
        => new($"{typeof(T).Name} kaydı bulunamadı. Id: {id}");
}
