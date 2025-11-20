using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EBOS.Core.Validators;

public static class BankAccountValidator
{
    private static readonly Regex IbanRegex =
        new("^[A-Z]{2}[A-Z0-9]{18,30}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool IsValidBankAccount(string ibanCode, string bankCode, string branchCode,
        string accountControl, string accountMiddle, string accountEnd)
    {
        if (string.IsNullOrWhiteSpace(ibanCode) || string.IsNullOrWhiteSpace(bankCode) || string.IsNullOrWhiteSpace(branchCode) ||
            string.IsNullOrWhiteSpace(accountControl) || string.IsNullOrWhiteSpace(accountMiddle) || string.IsNullOrWhiteSpace(accountEnd))
            return false;

        var fullAccount = (ibanCode + bankCode + branchCode + accountControl + accountMiddle + accountEnd)
            .Replace(" ", string.Empty, StringComparison.Ordinal)   // CA1307 corregido
            .ToUpperInvariant();

        if (!ValidateIban(fullAccount))
            return false;

        return ValidateControlDigits(bankCode, branchCode, accountControl, accountMiddle, accountEnd);
    }

    private static bool ValidateIban(string fullAccount)
    {
        if (!IsValidIbanFormat(fullAccount))
            return false;

        var rearranged = fullAccount[4..] + fullAccount[..4];
        var ibanNumericString = ConvertToIbanNumericString(rearranged);

        return VerifyIbanChecksum(ibanNumericString);
    }

    private static bool ValidateControlDigits(string bankCode, string branchCode, string accountControl, string accountMiddle, string accountEnd)
    {
        if (string.IsNullOrWhiteSpace(bankCode) || string.IsNullOrWhiteSpace(branchCode) || string.IsNullOrWhiteSpace(accountControl) ||
            string.IsNullOrWhiteSpace(accountMiddle) || string.IsNullOrWhiteSpace(accountEnd))
            return false;

        string controlDigits = accountControl.Length >= 2
            ? accountControl.AsSpan(0, 2).ToString()
            : "  ";

        string accountNumber = accountControl.Length >= 4
            ? string.Concat(accountControl.AsSpan(2, 2), accountMiddle, accountEnd)
            : string.Concat("  ", accountMiddle, accountEnd);

        var expectedControlDigits = CalculateAccountControlDigits(bankCode, branchCode, accountNumber);
        if (expectedControlDigits is null)
            return false;

        return controlDigits == expectedControlDigits;
    }

    private static bool VerifyIbanChecksum(string ibanNumericString)
    {
        if (string.IsNullOrEmpty(ibanNumericString))
            return false;

        var checksum = 0;

        foreach (var ch in ibanNumericString)
        {
            var digit = ch - '0';
            if (digit < 0 || digit > 9)
                return false;
            checksum = (checksum * 10 + digit) % 97;
        }

        return checksum == 1;
    }

    private static string ConvertToIbanNumericString(string ibanPart)
    {
        const int asciiShift = 55; // 'A' = 65 -> 65 - 55 = 10
        var builder = new StringBuilder(ibanPart.Length * 2);

        foreach (var character in ibanPart)
        {
            if (char.IsLetter(character))
            {
                var value = character - asciiShift;
                builder.Append(value.ToString(CultureInfo.InvariantCulture));
            }
            else
                builder.Append(character);
        }

        return builder.ToString();
    }

    private static bool IsValidIbanFormat(string bankAccount)
    {
        if (string.IsNullOrWhiteSpace(bankAccount))
            return false;

        return IbanRegex.IsMatch(bankAccount);
    }

    private static string? CalculateAccountControlDigits(string bankCode, string branchCode, string accountNumber)
    {
        var fullNumber = string.Concat(bankCode, branchCode, accountNumber);

        if (fullNumber.Length != 18)
            return null;

        // Simplifica el bucle usando LINQ (Any)
        if (fullNumber.Any(ch => !char.IsDigit(ch)))
            return null;

        var weights = new[] { 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
        var officePart = fullNumber.AsSpan(0, 8);
        var accountPart = fullNumber.AsSpan(8, 10);
        var sum1 = 0;

        for (var i = 0; i < officePart.Length; i++)
        {
            var digit = officePart[i] - '0';

            sum1 += digit * weights[i + 2]; // weights[2..9]
        }

        var remainder1 = sum1 % 11;
        var dc1Value = 11 - remainder1;
        var dc1 = dc1Value switch
        {
            11 => "0",
            10 => "1",
            _ => dc1Value.ToString(CultureInfo.InvariantCulture)
        };
        var sum2 = 0;

        for (var i = 0; i < accountPart.Length; i++)
        {
            var digit = accountPart[i] - '0';
            sum2 += digit * weights[i];
        }

        var remainder2 = sum2 % 11;
        var dc2Value = 11 - remainder2;
        var dc2 = dc2Value switch
        {
            11 => "0",
            10 => "1",
            _ => dc2Value.ToString(CultureInfo.InvariantCulture)
        };

        return dc1 + dc2;
    }
}