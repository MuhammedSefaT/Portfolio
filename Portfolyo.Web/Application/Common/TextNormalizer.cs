using System.Text;

namespace Portfolyo.Web.Application.Common;

/// <summary>
/// Karşılaştırma ve URL için metin normalleştirme.
/// </summary>
public static class TextNormalizer
{
    private static readonly Dictionary<char, char> TurkishMap = new()
    {
        ['ç'] = 'c', ['Ç'] = 'c',
        ['ğ'] = 'g', ['Ğ'] = 'g',
        ['ı'] = 'i', ['İ'] = 'i',
        ['ö'] = 'o', ['Ö'] = 'o',
        ['ş'] = 's', ['Ş'] = 's',
        ['ü'] = 'u', ['Ü'] = 'u'
    };

    /// <summary>"İçerik Editörü" -> "icerik-editoru"</summary>
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);

        foreach (var character in value.Trim())
        {
            if (TurkishMap.TryGetValue(character, out var replacement))
            {
                builder.Append(replacement);
            }
            else if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
            }
            else
            {
                // Boşluk, noktalama, eğik çizgi: hepsi ayraca dönüşür.
                builder.Append('-');
            }
        }

        var normalized = builder.ToString();

        // Tekrarlayan ayraçları teke indir.
        while (normalized.Contains("--"))
        {
            normalized = normalized.Replace("--", "-");
        }

        return normalized.Trim('-');
    }
}
