using EBOS.Core.Validators;

namespace EBOS.Core.Test.Validators;

public class BankAccountValidatorTests
{
    // Caso base válido que usaremos como referencia.
    // Corresponde al IBAN completo: ES82 0000 0000 0000 0000 0000
    private const string ValidIbanCode = "ES82";
    private const string ValidBankCode = "0000";
    private const string ValidBranchCode = "0000";
    private const string ValidAccountControl = "0000";
    private const string ValidAccountMiddle = "0000";
    private const string ValidAccountEnd = "0000";

    #region Argumentos nulos o en blanco
    [Theory]
    [InlineData(null, ValidBankCode, ValidBranchCode, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    [InlineData(ValidIbanCode, null, ValidBranchCode, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    [InlineData(ValidIbanCode, ValidBankCode, null, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    [InlineData(ValidIbanCode, ValidBankCode, ValidBranchCode, null, ValidAccountMiddle, ValidAccountEnd)]
    [InlineData(ValidIbanCode, ValidBankCode, ValidBranchCode, ValidAccountControl, null, ValidAccountEnd)]
    [InlineData(ValidIbanCode, ValidBankCode, ValidBranchCode, ValidAccountControl, ValidAccountMiddle, null)]
    [InlineData("   ", ValidBankCode, ValidBranchCode, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    [InlineData(ValidIbanCode, "   ", ValidBranchCode, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    [InlineData(ValidIbanCode, ValidBankCode, "   ", ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    [InlineData(ValidIbanCode, ValidBankCode, ValidBranchCode, "   ", ValidAccountMiddle, ValidAccountEnd)]
    [InlineData(ValidIbanCode, ValidBankCode, ValidBranchCode, ValidAccountControl, "   ", ValidAccountEnd)]
    [InlineData(ValidIbanCode, ValidBankCode, ValidBranchCode, ValidAccountControl, ValidAccountMiddle, "   ")]
    public void IsValidBankAccount_NullOrWhitespaceArguments_ReturnsFalse(string? ibanCode, string? bankCode, string? branchCode,
        string? accountControl, string? accountMiddle, string? accountEnd)
    {
        var result = BankAccountValidator.IsValidBankAccount(ibanCode!, bankCode!, branchCode!, 
            accountControl!, accountMiddle!, accountEnd!);

        Assert.False(result);
    }
    #endregion

    #region Caso completamente válido
    [Fact]
    public void IsValidBankAccount_ValidSpanishIbanAndCcc_ReturnsTrue()
    {
        var result = BankAccountValidator.IsValidBankAccount( ValidIbanCode, ValidBankCode, ValidBranchCode,
            ValidAccountControl, ValidAccountMiddle, ValidAccountEnd);

        Assert.True(result);
    }

    #endregion

    #region Casos inválidos (formato IBAN, checksum, CCC, caracteres no numéricos)

    [Theory]
    // IBAN con formato inválido (regex)
    // Caracter no alfanumérico en bankCode -> falla IbanRegex
    [InlineData(ValidIbanCode, "00-0", ValidBranchCode, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    // País no en formato "dos letras"
    [InlineData("E282", ValidBankCode, ValidBranchCode, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    // Demasiado corto como IBAN (longitud total < 20)
    [InlineData("ES82", "0", "0", "0", "0", "0")]
    // IBAN con checksum incorrecto (mod 97 != 1)
    [InlineData("ES00", ValidBankCode, ValidBranchCode, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    [InlineData("ES01", ValidBankCode, ValidBranchCode, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    // IBAN correcto pero CCC (dígitos de control) inválido
    // Mantiene el mismo IBAN completo que el válido, pero los datos para CCC provocan fallo:
    //   accountControl = "0" (1 dígito)
    //   accountMiddle  = "0000"
    //   accountEnd     = "000000"
    [InlineData(ValidIbanCode, ValidBankCode, ValidBranchCode, "0", "0000", "000000")]
    // CCC inválido por caracteres no numéricos en la parte bancaria
    [InlineData(ValidIbanCode, "00A0", ValidBranchCode, ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    [InlineData(ValidIbanCode, ValidBankCode, "00B0", ValidAccountControl, ValidAccountMiddle, ValidAccountEnd)]
    public void IsValidBankAccount_InvalidCases_ReturnsFalse(string ibanCode, string bankCode, string branchCode,
        string accountControl, string accountMiddle, string accountEnd)
    {
        var result = BankAccountValidator.IsValidBankAccount(ibanCode, bankCode,branchCode, accountControl, accountMiddle, accountEnd);

        Assert.False(result);
    }
    #endregion
}