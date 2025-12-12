using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EBOS.Core.Validators;

public static partial class BankAccountValidator
{
    private static readonly Regex IbanRegex = RegExIban();

    /// <summary>
    /// Valida una cuenta bancaria española construyendo el IBAN a partir del código de país (ibanCode)
    /// y las partes nacionales. Devuelve true si el IBAN calculado es válido y los dígitos de control nacionales coinciden.
    /// </summary>
    public static bool IsValidBankAccount(string ibanCode, string bankCode, string branchCode,
        string accountControl, string accountMiddle, string accountEnd)
    {
        if (string.IsNullOrWhiteSpace(ibanCode))
            return false;

        var country = ibanCode.Trim().ToUpperInvariant();

        // Actualmente soportamos solo España (ES). Si necesitas otros países, hay que añadir reglas BBAN por país.
        if (country != "ES")
            return false;

        // Normalizar y validar partes nacionales (quitar espacios y guiones)
        bankCode = (bankCode ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        branchCode = (branchCode ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        accountControl = (accountControl ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        accountMiddle = (accountMiddle ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        accountEnd = (accountEnd ?? string.Empty).Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);

        // Longitudes esperadas para España
        if (bankCode.Length != 4 || branchCode.Length != 4 || accountMiddle.Length != 4 || accountEnd.Length != 10)
            return false;

        if (accountControl.Length != 2 && accountControl.Length != 4)
            return false;

        // Construir BBAN (para España: bank(4)+branch(4)+control(2)+accountMiddle(4)+accountEnd(10))
        // Si accountControl tiene 4, sus últimos 2 dígitos forman parte del número de cuenta en el cálculo nacional.
        string bban = string.Concat(bankCode, branchCode, accountControl.Length >= 4 ? accountControl.Substring(2, 2) : string.Empty, accountMiddle, accountEnd);

        // Construir IBAN provisional con checksum "00"
        var provisionalIban = country + "00" + bban;

        // Calcular checksum IBAN
        var numeric = ConvertToIbanNumericString(provisionalIban);
        var checksum = CalculateIbanChecksum(numeric);
        if (checksum is null)
            return false;

        var iban = country + checksum + bban;

        // Validar formato IBAN y checksum
        if (!IbanRegex.IsMatch(iban))
            return false;

        if (!ValidateIbanChecksum(iban))
            return false;

        // Validar dígitos de control nacionales (DC)
        string controlDigits = accountControl.Length >= 2 ? accountControl[..2] : string.Empty;
        string accountNumber = accountControl.Length >= 4
            ? string.Concat(accountControl.Substring(2, 2), accountMiddle, accountEnd)
            : string.Concat(accountMiddle, accountEnd);

        var expectedDc = CalculateAccountControlDigits(bankCode, branchCode, accountNumber);
        if (expectedDc is null)
            return false;

        return controlDigits == expectedDc;
    }

    private static string? CalculateIbanChecksum(string numericRepresentation)
    {
        if (string.IsNullOrEmpty(numericRepresentation))
            return null;

        // Calcular resto módulo 97 de forma iterativa para evitar números grandes
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

        // Pesos estándar para España:
        var weightsOffice = new[] { 4, 8, 5, 10, 9, 7, 3, 6 };       // 8 dígitos (entidad+oficina)
        var weightsAccount = new[] { 1, 2, 4, 8, 5, 10, 9, 7, 3, 6 }; // 10 dígitos (número de cuenta)

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
    // IBAN general: 2 letras (país) + 2 dígitos (checksum) + BBAN (hasta 30 caracteres)

    [GeneratedRegex("^[A-Z]{2}\\d{2}[A-Z0-9]{11,30}$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex RegExIban();
}