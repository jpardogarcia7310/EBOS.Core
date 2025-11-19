using System.Globalization;

namespace EBOS.Core.Extensions;

public static class StringExtensions
{
    public static DateTime? ToDateFromStringIso8601Format(this string dateAsString)
    {
        return string.IsNullOrWhiteSpace(dateAsString) ? null : ParseToDate(dateAsString, "yyyyMMdd");
    }

    public static DateTime? ToDateFromStringWithHHMM(this string dateAsString)
    {
        return string.IsNullOrWhiteSpace(dateAsString) ? null : ParseToDate(dateAsString, "yyyyMMddHHmm");
    }

    private static DateTime? ParseToDate(string dateToParse, string format)
    {
        return DateTime.TryParseExact(dateToParse, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : (DateTime?)null;
    }

    public static string RemoveSpecialCharacters(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        return text.Replace("\n", string.Empty, StringComparison.Ordinal)
                   .Replace("\r", string.Empty, StringComparison.Ordinal)
                   .Replace("\r\n", string.Empty, StringComparison.Ordinal)
                   .Replace("\t", string.Empty, StringComparison.Ordinal)
                   .Replace("\"", string.Empty, StringComparison.Ordinal)
                   .Replace(",", string.Empty, StringComparison.Ordinal)
                   .Replace("'", string.Empty, StringComparison.Ordinal)
                   .Replace("&", string.Empty, StringComparison.Ordinal)
                   .Replace(Environment.NewLine, string.Empty, StringComparison.Ordinal);
    }
}
