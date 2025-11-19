using System.Globalization;
using System.Text.RegularExpressions;

namespace EBOS.Core.Extensions;

public static class DateExtensions
{
    public static string ToDateFormat(this DateTime? date)
    {
        return date?.ToString("yyyyMMdd", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public static string ToDateFormatWithTime(this DateTime? date)
    {
        return date?.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public static string ToDateFormatWithTime(this DateTime date)
    {
        return date.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public static string ToDateFormatWithTimeISO8601(this DateTime date)
    {
        return date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public static DateTime? ParsePart(string s, string regex, string format, IFormatProvider provider, DateTime? defaultValue)
    {
        try
        {
            var match = Regex.Match(s, regex);
            if (match.Success)
            {
                var value = match.Groups[0]?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    return DateExtensions.ParseExactOrDefault(value, format, provider, defaultValue);
                }
                else
                {
                    return defaultValue ?? default(DateTime?);
                }
            }
            else
            {
                return defaultValue ?? default(DateTime?);
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return defaultValue ?? default(DateTime?);
        }
        catch (ArgumentException)
        {
            return defaultValue ?? default(DateTime?);
        }
    }

    public static DateTime? ParseExactOrDefault(string s, string format, IFormatProvider provider, DateTime? defaultValue)
    {
        try
        {
            return DateTime.ParseExact(s, format, provider);
        }
        catch (FormatException)
        {
            return defaultValue ?? default(DateTime?);
        }
        catch (ArgumentNullException)
        {
            return defaultValue ?? default(DateTime?);
        }
        catch (ArgumentException)
        {
            return defaultValue ?? default(DateTime?);
        }
    }
}
