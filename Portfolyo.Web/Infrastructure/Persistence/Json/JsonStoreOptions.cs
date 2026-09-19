using System.Text.Json;

namespace Portfolyo.Web.Infrastructure.Persistence.Json;

/// <summary>
/// JSON dosya deposunun yapılandırması.
/// </summary>
public class JsonStoreOptions
{
    /// <summary>
    /// JSON dosyalarının bulunduğu mutlak klasör yolu.
    /// DI kaydı sırasında ContentRootPath ile birleştirilerek doldurulur.
    /// </summary>
    public string RootPath { get; set; } = string.Empty;

    /// <summary>
    /// Dosyalar elle de okunabilsin diye girintili ve camelCase yazılır.
    /// Enum'lar sayı olarak saklanır; bu yüzden Permission değerleri sabit kalmalıdır.
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; set; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
}
