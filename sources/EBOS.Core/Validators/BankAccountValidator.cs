using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EBOS.Core.Validators;

public static partial class BankAccountValidator
{
    private static readonly Regex IbanRegex = RegExIban();

    /// <summary>
    /// Validates a Spanish bank account by building the IBAN from the country code (ibanCode)
    /// and the national parts. Returns true when the calculated IBAN is valid and the national
    /// control digits match.
    /// </summary>
    public static bool IsValidBankAccount(string ibanCode, string bankCode, string branchCode,
        string accountControl, string accountMiddle, string accountEnd)
    {
        if (string.IsNullOrWhiteSpace(ibanCode))
            return false;

        var ibanPart = ibanCode.Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();

        string country;
        string? expectedChecksum = null;
        if (ibanPart.Length == 2)
        {
            country = ibanPart;
        }
        else if (ibanPart.Length == 4)
        {
            country = ibanPart[..2];
            expectedChecksum = ibanPart.Substring(2, 2);
        }
        else
        {
            return false;
        }

        // Only Spain (ES) is supported right now. Add BBAN rules per country for other cases.
        if (country != "ES")
            return false;

        // Normalize and validate national parts (remove spaces and hyphens).
        bankCode = (bankCode ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        branchCode = (branchCode ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        accountControl = (accountControl ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        accountMiddle = (accountMiddle ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        accountEnd = (accountEnd ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);

        // Expected lengths for Spain.
        if (bankCode.Length != 4 || branchCode.Length != 4)
            return false;

        if (accountControl.Length != 2)
            return false;

        // Build the bank account (10 digits): accountMiddle(4) + accountEnd(6) or accountEnd(10).
        string accountNumber;
        if (accountMiddle.Length == 0 && accountEnd.Length == 10)
        {
            accountNumber = accountEnd;
        }
        else if (accountMiddle.Length == 4 && accountEnd.Length == 6)
        {
            accountNumber = string.Concat(accountMiddle, accountEnd);
        }
        else
        {
            return false;
        }

        // Build BBAN (Spain: bank(4)+branch(4)+control(2)+account(10)).
        string bban = string.Concat(bankCode, branchCode, accountControl, accountNumber);

        // Compute IBAN checksum: move country+checksum to the end before calculating mod 97.
        var rearranged = bban + country + "00";
        var numeric = ConvertToIbanNumericString(rearranged);
        var checksum = CalculateIbanChecksum(numeric);
        if (checksum is null)
            return false;

        var iban = country + checksum + bban;
        if (expectedChecksum is not null && !string.Equals(expectedChecksum, checksum, StringComparison.Ordinal))
            return false;

        // Validate IBAN format and checksum.
        if (!IbanRegex.IsMatch(iban))
            return false;

        if (!ValidateIbanChecksum(iban))
            return false;

        // Validate national control digits (DC).
        string controlDigits = accountControl;
        var expectedDc = CalculateAccountControlDigits(bankCode, branchCode, accountNumber);
        if (expectedDc is null)
            return false;

        return controlDigits == expectedDc;
    }

    private static string? CalculateIbanChecksum(string numericRepresentation)
    {
        if (string.IsNullOrEmpty(numericRepresentation))
            return null;

        // Compute mod 97 iteratively to avoid large numbers.
        var remainder = 0;
        foreach (var ch in numericRepresentation)
        {
            if (ch < '0' || ch > '9')
                return null;
            remainder = (remainder * 10 + (ch - '0')) % 97;
        }

        var checkValue = 98 - remainder;
        return checkValue.ToString("D2", CultureInfo.InvariantCulture);
    }

    private static bool ValidateIbanChecksum(string iban)
    {
        if (string.IsNullOrWhiteSpace(iban) || iban.Length < 4)
            return false;

        var rearranged = iban[4..] + iban[..4];
        var numeric = ConvertToIbanNumericString(rearranged);

        var remainder = 0;
        foreach (var ch in numeric)
        {
            if (ch < '0' || ch > '9')
                return false;
            remainder = (remainder * 10 + (ch - '0')) % 97;
        }

        return remainder == 1;
    }

    private static string ConvertToIbanNumericString(string input)
    {
        // A -> 10, B -> 11, ... Z -> 35
        var sb = new StringBuilder(input.Length * 2);
        foreach (var ch in input.ToUpperInvariant())
        {
            if (ch >= 'A' && ch <= 'Z')
            {
                sb.Append((ch - 'A' + 10).ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }

    private static string? CalculateAccountControlDigits(string bankCode, string branchCode, string accountNumber)
    {
        var fullNumber = string.Concat(bankCode, branchCode, accountNumber);

        if (fullNumber.Length != 18)
            return null;

        if (fullNumber.Any(ch => ch < '0' || ch > '9'))
            return null;

        // Standard weights for Spain:
        var weightsOffice = new[] { 4, 8, 5, 10, 9, 7, 3, 6 };       // 8 digits (bank + branch)
        var weightsAccount = new[] { 1, 2, 4, 8, 5, 10, 9, 7, 3, 6 }; // 10 digits (account number)

        var officePart = fullNumber.AsSpan(0, 8);
        var accountPart = fullNumber.AsSpan(8, 10);

        int sumOffice = 0;
        for (int i = 0; i < officePart.Length; i++)
        {
            sumOffice += (officePart[i] - '0') * weightsOffice[i];
        }

        int remainderOffice = sumOffice % 11;
        int dc1Value = 11 - remainderOffice;
        string dc1 = dc1Value switch
        {
            11 => "0",
            10 => "1",
            _ => dc1Value.ToString(CultureInfo.InvariantCulture)
        };

        int sumAccount = 0;
        for (int i = 0; i < accountPart.Length; i++)
        {
            sumAccount += (accountPart[i] - '0') * weightsAccount[i];
        }

        int remainderAccount = sumAccount % 11;
        int dc2Value = 11 - remainderAccount;
        string dc2 = dc2Value switch
        {
            11 => "0",
            10 => "1",
            _ => dc2Value.ToString(CultureInfo.InvariantCulture)
        };

        return dc1 + dc2;
    }
    // General IBAN: 2 letters (country) + 2 digits (checksum) + BBAN (up to 30 chars).

    [GeneratedRegex("^[A-Z]{2}\\d{2}[A-Z0-9]{11,30}$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex RegExIban();
}
