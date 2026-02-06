using EBOS.Core.Validators;

namespace EBOS.Core.Test.Validators;

public class EmailValidatorTests
{
    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("user.name+tag+sorting@example.com", true)]
    [InlineData("user@sub.example.co.uk", true)]
    [InlineData("  user@example.com  ", true)]
    public void IsValidEmail_ValidAddresses(string email, bool expected)
    {
        Assert.Equal(expected, EmailValidator.IsValidEmail(email));
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("plainaddress", false)]
    [InlineData("user@.com", false)]
    [InlineData("user@com", false)]
    [InlineData("user@@example.com", false)]
    [InlineData("user@ex ample.com", false)]
    public void IsValidEmail_InvalidAddresses(string email, bool expected)
    {
        Assert.Equal(expected, EmailValidator.IsValidEmail(email));
    }
}

