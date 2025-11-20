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
        string[] lastLetters =
        [
            "A", "B", "C", "D", "E", "F", "G", "H",
        "I", "J", "K", "L", "M", "N", "0"
        ];

        // CORREGIDO: longitud debe ser exactamente 9
        if (string.IsNullOrWhiteSpace(identification) || identification.Length != 9)
            return false;
        identification = identification.ToUpperInvariant();

        int evenSum = 0;
        int oddSum = 0;
        // Último carácter (letra o dígito de control)
        char lastCharacter = identification[8];

        // Recorremos posiciones 1,3,5 (índices 1..6, de dos en dos)
        for (int index = 1; index < 7; index += 2)
        {
            // Dígito en posición impar (según la norma CIF)
            int oddDigit = identification[index] - '0';

            if (oddDigit < 0 || oddDigit > 9)
                return false;

            int doubled = 2 * oddDigit;

            oddSum += (doubled / 10) + (doubled % 10);

            // Dígito en posición par siguiente
            int evenIndex = index + 1;
            int evenDigit = identification[evenIndex] - '0';

            if (evenDigit < 0 || evenDigit > 9)
                return false;
            evenSum += evenDigit;
        }

        int totalSum = evenSum + oddSum;
        int lastDigitOfSum = totalSum % 10;
        int number = (10 - lastDigitOfSum) % 10; // si lastDigitOfSum == 0 => 0
        bool matchNumeric = lastCharacter == (char)('0' + number);
        bool matchLetter;

        if (number == 0)
            matchLetter = lastCharacter == lastLetters[^1][0];
        else
            matchLetter = lastCharacter == lastLetters[number - 1][0];

        return matchNumeric || matchLetter;
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
        if (string.IsNullOrWhiteSpace(identification) || (identification.Length != 9 && identification.Length != 11))
            return false;

        string identificationNumbers = identification[1..^1];
        char firstLetter = identification[0];
        char lastLetter = identification[^1];

        // NUEVO: solo se permiten X, Y, Z
        if (firstLetter != 'X' && firstLetter != 'Y' && firstLetter != 'Z')
            return false;

        bool numbersValid = int.TryParse(identificationNumbers, out int identificationNumberInteger);

        if (!numbersValid)
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
        bool numbersValid = int.TryParse(identificationNumbers,  NumberStyles.Integer,
            CultureInfo.InvariantCulture, out int identificationNumberInteger);

        if (!numbersValid)
            return false;

        return CalculateDniLetter(identificationNumberInteger)[0] == identificationLetter;
    }
}