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
        // Known valid Spanish IBAN example
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
        // invalid lengths for parts should return false
        var result = BankAccountValidator.IsValidBankAccount("ES", "21", "0418", "45", "0200", "051332");
        Assert.False(result);
    }
}