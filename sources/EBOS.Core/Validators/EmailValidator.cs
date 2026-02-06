using System.Net.Mail;
using System.Text.RegularExpressions;

namespace EBOS.Core.Validators;

public static partial class EmailValidator
{
    private static readonly Regex SimpleDomainRegex =
        DomainRegEx();

    public static bool IsValidEmail(string? emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return false;

        var trimmed = emailAddress.Trim();

        if (!MailAddress.TryCreate(trimmed, out _))
            return false;

        return SimpleDomainRegex.IsMatch(trimmed);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "es-ES")]
    private static partial Regex DomainRegEx();
}