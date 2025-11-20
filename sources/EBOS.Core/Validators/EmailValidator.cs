using System.Text.RegularExpressions;

namespace EBOS.Core.Validators;

public static class EmailValidator
{
    private static readonly Regex ValidEmailRegex = CreateValidEmailRegex();

    public static bool IsValidEmail(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            return false;

        return ValidEmailRegex.IsMatch(emailAddress);
    }

    private static Regex CreateValidEmailRegex()
    {
        const string validEmailPattern =
            @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" +
            @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" +
            @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        return new Regex(validEmailPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
