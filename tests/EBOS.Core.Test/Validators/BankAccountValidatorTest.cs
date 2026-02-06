using EBOS.Core.Validators;

namespace EBOS.Core.Test.Validators;

public class BankAccountValidatorTests
{
    [Fact]
    public void IsValidBankAccount_ReturnsFalse_WhenCountryIsNotSupported()
    {
        var result = BankAccountValidator.IsValidBankAccount("FR", "2100", "0418", "45", "0200", "051332");
        Assert.False(result);
    }

    [Theory]
    [InlineData(null, "2100", "0418", "45", "0200", "051332")]
    [InlineData("", "2100", "0418", "45", "0200", "051332")]
    [InlineData("  ", "2100", "0418", "45", "0200", "051332")]
    public void IsValidBankAccount_ReturnsFalse_WhenCountryIsNullOrWhitespace(string? country, string bank, string branch, string control, string middle, string end)
    {
        var result = BankAccountValidator.IsValidBankAccount(country!, bank, branch, control, middle, end);
        Assert.False(result);
    }

    [Fact]
    public void ValidateIbanChecksum_KnownValidIban_ReturnsTrue_UsingReflection()
    {
        // Known valid Spanish IBAN example.
        const string iban = "ES9121000418450200051332";
        var type = typeof(BankAccountValidator);
        var invoked = ReflectionHelper.InvokeStaticNonPublicMethod(type, "ValidateIbanChecksum", [iban]);
        Assert.IsType<bool>(invoked);
        Assert.True((bool)invoked!);
    }

    [Fact]
    public void CalculateAccountControlDigits_KnownSpanishAccount_ReturnsExpected_UsingReflection()
    {
        // For IBAN ES91 2100 0418 45 0200051332
        // bankCode = 2100, branchCode = 0418, accountNumber = 0200051332 -> expected DC = "45"
        var type = typeof(BankAccountValidator);
        var result = ReflectionHelper.InvokeStaticNonPublicMethod(type, "CalculateAccountControlDigits", ["2100", "0418", "0200051332"]);
        Assert.NotNull(result);
        Assert.Equal("45", result);
    }

    [Fact]
    public void IsValidBankAccount_ReturnsFalse_OnInvalidParts()
    {
        // Invalid lengths for parts should return false.
        var result = BankAccountValidator.IsValidBankAccount("ES", "21", "0418", "45", "0200", "051332");
        Assert.False(result);
    }

    [Theory]
    // Examples of Spanish IBANs published in documentation.
    [InlineData("ES9121000418450200051332")]
    [InlineData("ES7921000813610123456789")]
    public void ValidateIbanChecksum_KnownValidIbans_ReturnsTrue(string iban)
    {
        var type = typeof(BankAccountValidator);
        var invoked = ReflectionHelper.InvokeStaticNonPublicMethod(type, "ValidateIbanChecksum", [iban]);
        Assert.IsType<bool>(invoked);
        Assert.True((bool)invoked!);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("123")]
    public void IsValidBankAccount_ReturnsFalse_WhenControlLengthIsInvalid(string control)
    {
        var result = BankAccountValidator.IsValidBankAccount("ES", "2100", "0418", control, "0200", "051332");
        Assert.False(result);
    }

    [Fact]
    public void IsValidBankAccount_ReturnsFalse_WhenPartsContainNonDigits()
    {
        var result = BankAccountValidator.IsValidBankAccount("ES", "21A0", "0418", "45", "0200", "051332");
        Assert.False(result);
    }

    [Theory]
    [InlineData("ES91", "2100", "0418", "45", "", "0200051332")]
    [InlineData("ES79", "2100", "0813", "61", "", "0123456789")]
    [InlineData("ES91", "2100", "0418", "45", "0200", "051332")]
    public void IsValidBankAccount_ReturnsTrue_ForValidSpanishAccounts_WithIbanCode(string ibanCode, string bank, string branch, string control, string middle, string end)
    {
        var result = BankAccountValidator.IsValidBankAccount(ibanCode, bank, branch, control, middle, end);
        Assert.True(result);
    }

    [Fact]
    public void IsValidBankAccount_ReturnsFalse_WhenIbanChecksumDoesNotMatch()
    {
        var result = BankAccountValidator.IsValidBankAccount("ES00", "2100", "0418", "45", "", "0200051332");
        Assert.False(result);
    }
}
