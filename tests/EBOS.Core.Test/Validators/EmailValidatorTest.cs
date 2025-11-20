using EBOS.Core.Validators;

namespace EBOS.Core.Test.Validators;

public class EmailValidatorTests
{
    #region Null / empty / whitespace
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void IsValidEmail_NullEmptyOrWhitespace_ReturnsFalse(string? email)
    {
        // El método admite null lógicamente, aunque la firma sea string.
        bool result = EmailValidator.IsValidEmail(email!);

        Assert.False(result);
    }
    #endregion

    #region Direcciones válidas
    [Theory]
    // Formato básico
    [InlineData("simple@example.com")]
    // Subdominios
    [InlineData("user@mail.example.com")]
    // Símbolos permitidos en la parte local
    [InlineData("user.name+tag@example.co.uk")]
    [InlineData("user_name-123@example.org")]
    [InlineData("customer/department=shipping@example.com")]
    [InlineData("!def!xyz%abc@example.com")]
    [InlineData("user%example@example.org")]
    // Parte local entre comillas (permite espacios y puntos consecutivos)
    [InlineData("\"John..Doe\"@example.com")]
    [InlineData("\"much.more unusual\"@example.com")]
    // Mayúsculas (el regex es IgnoreCase)
    [InlineData("USER@EXAMPLE.COM")]
    [InlineData("User.Name+Tag@Sub.Example.CoM")]
    public void IsValidEmail_ValidAddresses_ReturnsTrue(string email)
    {
        bool result = EmailValidator.IsValidEmail(email);

        Assert.True(result);
    }
    #endregion

    #region Direcciones inválidas
    [Theory]
    // Falta '@'
    [InlineData("plainaddress")]
    [InlineData("user.example.com")]
    // Más de un '@'
    [InlineData("user@@example.com")]
    [InlineData("user@sub@@example.com")]
    // Parte local inválida
    [InlineData(".user@example.com")]      // punto inicial no permitido
    [InlineData("user.@example.com")]      // punto final no permitido
    [InlineData("user..name@example.com")] // puntos consecutivos sin comillas
    [InlineData("user..name@sub.example.com")]
    // Caracteres no permitidos en la parte local
    [InlineData("user(name)@example.com")]
    [InlineData("user<>@example.com")]
    // Dominio inválido
    [InlineData("user@.example.com")]   // dominio empieza con punto
    [InlineData("user@example.com.")]   // dominio termina con punto
    [InlineData("user@-example.com")]   // dominio empieza con guion
    [InlineData("user@example-.com")]   // parte del dominio termina con guion
    [InlineData("user@example")]        // sin TLD
    [InlineData("user@example.c")]      // TLD de 1 carácter
    [InlineData("user@example.123")]    // TLD solo numérico
    // Espacios no permitidos sin comillas
    [InlineData("user name@example.com")]
    [InlineData(" user@example.com")]
    [InlineData("user@example.com ")]
    // Formato de display name no soportado por el regex
    [InlineData("John Doe <user@example.com>")]
    public void IsValidEmail_InvalidAddresses_ReturnsFalse(string email)
    {
        bool result = EmailValidator.IsValidEmail(email);

        Assert.False(result);
    }
    #endregion
}






