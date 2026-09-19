namespace Portfolyo.Web.Application.Common.Results;

/// <summary>
/// Veri döndürmeyen işlemlerin sonucu. Servisler hata durumunda exception yerine
/// bu tipi döner; beklenen hatalar akışın normal parçasıdır.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, ResultStatus status, string? message, IReadOnlyList<ResultError>? errors)
    {
        IsSuccess = isSuccess;
        Status = status;
        Message = message;
        Errors = errors ?? Array.Empty<ResultError>();
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ResultStatus Status { get; }

    public string? Message { get; }

    /// <summary>Doğrulama hataları; başarılı sonuçta boştur.</summary>
    public IReadOnlyList<ResultError> Errors { get; }

    public static Result Success(string? message = null)
        => new(true, ResultStatus.Success, message, null);

    public static Result NotFound(string message = "Kayıt bulunamadı.")
        => new(false, ResultStatus.NotFound, message, null);

    public static Result Invalid(IReadOnlyList<ResultError> errors, string message = "Girilen bilgiler geçersiz.")
        => new(false, ResultStatus.ValidationError, message, errors);

    public static Result Invalid(string propertyName, string message)
        => Invalid(new[] { new ResultError(propertyName, message) }, message);

    public static Result Conflict(string message)
        => new(false, ResultStatus.Conflict, message, null);

    public static Result Forbidden(string message = "Bu işlem için yetkiniz yok.")
        => new(false, ResultStatus.Forbidden, message, null);

    public static Result Failure(string message)
        => new(false, ResultStatus.Error, message, null);

    /// <summary>
    /// Başarısız bir sonucu, veri döndüren başka bir sonuca taşır.
    /// Servisler arası çağrılarda hata bilgisi kaybolmaz.
    /// </summary>
    public Result<TOther> CarryFailure<TOther>()
        => Result<TOther>.FromFailure(this);
}
