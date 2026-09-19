using Portfolyo.Web.Application.Common;

namespace Portfolyo.Web.Tests.Common;

public class TextNormalizerTests
{
    [Theory]
    [InlineData("İçerik Editörü", "icerik-editoru")]
    [InlineData("  Süper   Yönetici  ", "super-yonetici")]
    [InlineData("Sıfır/Gün", "sifir-gun")]
    [InlineData("ÇĞİÖŞÜ", "cgiosu")]
    [InlineData("admin", "admin")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void Normalize_Beklenen_Ciktiyi_Uretir(string? girdi, string beklenen)
        => Assert.Equal(beklenen, TextNormalizer.Normalize(girdi));
}
