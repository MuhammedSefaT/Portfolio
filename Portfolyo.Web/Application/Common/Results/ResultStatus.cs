namespace Portfolyo.Web.Application.Common.Results;

/// <summary>
/// İşlem sonucunun türü. Controller bu değere bakıp doğru HTTP davranışını seçer.
/// </summary>
public enum ResultStatus
{
    Success = 0,

    /// <summary>Kayıt bulunamadı.</summary>
    NotFound = 1,

    /// <summary>Girdi doğrulamadan geçemedi; ayrıntı Errors içinde.</summary>
    ValidationError = 2,

    /// <summary>Kural çakışması (ör. aynı kullanıcı adı zaten var).</summary>
    Conflict = 3,

    /// <summary>Yetki yok.</summary>
    Forbidden = 4,

    /// <summary>Beklenmeyen hata.</summary>
    Error = 5
}
