namespace Portfolyo.Web.Application.Common.Results;

/// <summary>
/// Tek bir doğrulama hatası. PropertyName, formdaki alan adıyla eşleşir;
/// böylece hata mesajı doğru alanın altında gösterilebilir.
/// </summary>
public record ResultError(string PropertyName, string Message);
