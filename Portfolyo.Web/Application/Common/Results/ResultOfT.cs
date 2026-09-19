namespace Portfolyo.Web.Application.Common.Results;

/// <summary>
/// Veri döndüren işlemlerin sonucu. Başarılıysa Data doludur,
/// başarısızsa Data null'dur ve Status hatanın türünü söyler.
/// </summary>
public class Result<T> : Result
{
    private Result(bool isSuccess, ResultStatus status, T? data, string? message, IReadOnlyList<ResultError>? errors)
        : base(isSuccess, status, message, errors)
        => Data = data;

    public T? Data { get; }

    public static Result<T> Success(T data, string? message = null)
        => new(true, ResultStatus.Success, data, message, null);

    public static new Result<T> NotFound(string message = "Kayıt bulunamadı.")
        => new(false, ResultStatus.NotFound, default, message, null);

    public static new Result<T> Invalid(IReadOnlyList<ResultError> errors, string message = "Girilen bilgiler geçersiz.")
        => new(false, ResultStatus.ValidationError, default, message, errors);

    public static new Result<T> Invalid(string propertyName, string message)
        => Invalid(new[] { new ResultError(propertyName, message) }, message);

    public static new Result<T> Conflict(string message)
        => new(false, ResultStatus.Conflict, default, message, null);

    public static new Result<T> Forbidden(string message = "Bu işlem için yetkiniz yok.")
        => new(false, ResultStatus.Forbidden, default, message, null);

    public static new Result<T> Failure(string message)
        => new(false, ResultStatus.Error, default, message, null);

    /// <summary>Başarısız bir sonucu bu veri tipine taşır. Result.CarryFailure kullanır.</summary>
    internal static Result<T> FromFailure(Result failure)
        => new(false, failure.Status, default, failure.Message, failure.Errors);
}
