using EBOS.Core.Enums;
using System.Globalization;

namespace EBOS.Core.Validators;

public static class SpanishIdentificationValidator
{
    public static bool IsValidIdentification(string identificationNumber, SpanishIdentificationType identificationType)
    {
        if (string.IsNullOrWhiteSpace(identificationNumber))
            return false;

        identificationNumber = identificationNumber.ToUpperInvariant();

        return identificationType switch
        {
            SpanishIdentificationType.DNI => ValidateDni(identificationNumber),
            SpanishIdentificationType.NIE => ValidateNie(identificationNumber),
            SpanishIdentificationType.CIF => ValidateCif(identificationNumber),
            _ => false
        };
    }

    private static bool ValidateCif(string identification)
    {
        if (string.IsNullOrWhiteSpace(identification) || identification.Length != 9)
            return false;

        identification = identification.ToUpperInvariant();

        // Los caracteres 1..7 deben ser dígitos
        for (int i = 1; i <= 7; i++)
        {
            if (identification[i] < '0' || identification[i] > '9')
                return false;
        }

        int sumEven = 0; // posiciones 2,4,6 (índices 2,4,6)
        int sumOdd = 0;  // posiciones 1,3,5,7 (índices 1,3,5,7) -> doble y sumar dígitos

        for (int i = 1; i <= 7; i++)
        {
            int digit = identification[i] - '0';
            if ((i % 2) == 0)
            {
                // posición par
                sumEven += digit;
            }
            else
            {
                // posición impar: doble y sumar dígitos
                int doubled = digit * 2;
                sumOdd += (doubled / 10) + (doubled % 10);
            }
        }

        int total = sumEven + sumOdd;
        int controlDigit = (10 - (total % 10)) % 10;

        char controlChar = identification[8];

        // Coincide con dígito de control numérico?
        if (controlChar == (char)('0' + controlDigit))
            return true;

        // Coincide con letra de control (tabla estándar)
        const string controlLetters = "JABCDEFGHI"; // índice = controlDigit
        if (controlChar == controlLetters[controlDigit])
            return true;

        return false;
    }

    private static string CalculateDniLetter(int dniNumbers)
    {
        string[] controlLetters =
        [
            "T", "R", "W", "A", "G", "M", "Y", "F",
            "P", "D", "X", "B", "N", "J", "Z", "S",
            "Q", "V", "H", "L", "C", "K", "E"
        ];
        int mod = dniNumbers % 23;

        return controlLetters[mod];
    }

    private static bool ValidateNie(string identification)
    {
        if (string.IsNullOrWhiteSpace(identification) || identification.Length != 9)
            return false;

        string identificationNumbers = identification[1..^1];
        char firstLetter = identification[0];
        char lastLetter = identification[^1];

        if (firstLetter != 'X' && firstLetter != 'Y' && firstLetter != 'Z')
            return false;

        if (!int.TryParse(identificationNumbers, out int identificationNumberInteger))
            return false;

        if (firstLetter == 'Y')
            identificationNumberInteger += 10000000;
        else if (firstLetter == 'Z')
            identificationNumberInteger += 20000000;

        return CalculateDniLetter(identificationNumberInteger)[0] == lastLetter;
    }

    private static bool ValidateDni(string identification)
    {
        if (string.IsNullOrWhiteSpace(identification) || identification.Length != 9)
            return false;

        string identificationNumbers = identification[..^1];
        char identificationLetter = identification[^1];

        if (!int.TryParse(identificationNumbers, NumberStyles.Integer, CultureInfo.InvariantCulture, out int identificationNumberInteger))
            return false;

        return CalculateDniLetter(identificationNumberInteger)[0] == identificationLetter;
    }
}
