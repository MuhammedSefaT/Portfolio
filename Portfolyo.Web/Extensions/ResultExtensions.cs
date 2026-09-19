using Microsoft.AspNetCore.Mvc.ModelBinding;
using Portfolyo.Web.Application.Common.Results;

namespace Portfolyo.Web.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Başarısız bir sonucun hatalarını forma taşır.
    /// Alan adı olan hatalar ilgili alanın altında, olmayanlar formun genel hatası olarak görünür.
    /// </summary>
    public static void AddToModelState(this Result result, ModelStateDictionary modelState)
    {
        if (result.IsSuccess)
        {
            return;
        }

        if (result.Errors.Count == 0)
        {
            modelState.AddModelError(string.Empty, result.Message ?? "İşlem tamamlanamadı.");
            return;
        }

        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName ?? string.Empty, error.Message);
        }
    }
}
