using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace JewelryStore.Modules.Catalog.Domain;

internal static partial class Slugifier
{
    public static string Create(string value, Guid? suffix = null)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        var slug = NonAlphaNumeric().Replace(builder.ToString().ToLowerInvariant(), "-").Trim('-');
        return suffix is null ? slug : $"{slug}-{suffix.Value.ToString("N")[..8]}";
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonAlphaNumeric();
}

