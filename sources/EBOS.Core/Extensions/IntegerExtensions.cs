using System.Globalization;

namespace EBOS.Core.Extensions;

public static class IntegerExtensions
{
    public static int? ParseOrDefault(string s, int? defaultValue)
    {
        try
        {
            return int.Parse(s, CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            return defaultValue ?? default(int?);
        }
        catch (OverflowException)
        {
            return defaultValue ?? default(int?);
        }
    }
}
