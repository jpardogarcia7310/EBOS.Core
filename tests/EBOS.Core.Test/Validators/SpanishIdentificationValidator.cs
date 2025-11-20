using EBOS.Core.Enums;
using EBOS.Core.Validators;

namespace EBOS.Core.Test.Validators;

public class SpanishIdentificationValidatorTests
{
    #region DNI
    [Theory]
    // 12345678 % 23 = 14 -> Z
    [InlineData("12345678Z")]
    // 1 % 23 = 1 -> R
    [InlineData("00000001R")]
    // 99999999 % 23 = 1 -> R
    [InlineData("99999999R")]
    // letra en minúscula
    [InlineData("12345678z")]
    public void IsValidIdentification_ValidDni_ReturnsTrue(string dni)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(dni, SpanishIdentificationType.DNI);

        Assert.True(result);
    }

    [Theory]
    // Letra incorrecta
    [InlineData("12345678A")]
    [InlineData("00000001A")]
    [InlineData("99999999A")]
    // Longitud incorrecta
    [InlineData("1234567Z")]      // pocos dígitos
    [InlineData("123456789Z")]    // demasiados dígitos
    [InlineData("1234567")]       // sin letra
                                  // Vacío o espacios
    [InlineData("")]
    [InlineData("   ")]
    // No numéricos en la parte numérica
    [InlineData("ABCDEFGHZ")]
    [InlineData("1234A678Z")]
    public void IsValidIdentification_InvalidDni_ReturnsFalse(string dni)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(dni, SpanishIdentificationType.DNI);

        Assert.False(result);
    }
    #endregion

    #region NIE
    [Theory]
    [InlineData("X1234567L")]
    [InlineData("Y1234567X")]
    [InlineData("Z1234567R")]
    [InlineData("x1234567l")] // minúsculas
    public void IsValidIdentification_ValidNie_ReturnsTrue(string nie)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(nie, SpanishIdentificationType.NIE);

        Assert.True(result);
    }

    [Theory]
    // Letra de control incorrecta
    [InlineData("X1234567A")]
    [InlineData("Y1234567A")]
    [InlineData("Z1234567A")]
    // Primera letra no X/Y/Z
    [InlineData("A1234567L")]
    [InlineData("K1234567L")]
    // Longitud incorrecta o vacío
    [InlineData("X123456L")]        // pocos dígitos
    [InlineData("X12345678L")]      // longitud 10
    [InlineData("X1234567890L")]    // longitud > 11
    [InlineData("")]
    [InlineData("   ")]
    // No numéricos en la parte numérica
    [InlineData("X1234A67L")]
    [InlineData("YAAAAAAAQ")]
    public void IsValidIdentification_InvalidNie_ReturnsFalse(string nie)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(nie, SpanishIdentificationType.NIE);

        Assert.False(result);
    }
    #endregion

    #region CIF
    [Theory]
    // CIF muy usado como ejemplo
    [InlineData("A58818501")]
    [InlineData("a58818501")] // minúsculas
    public void IsValidIdentification_ValidCif_ReturnsTrue(string cif)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(cif, SpanishIdentificationType.CIF);

        Assert.True(result);
    }

    [Theory]
    // Dígito / letra de control incorrectos
    [InlineData("A58818502")]
    // Longitud incorrecta o vacío
    [InlineData("A5881850")]    // longitud 8
    [InlineData("A588185012")]  // longitud 10
    [InlineData("")]
    [InlineData("   ")]
    // No numéricos en parte numérica
    [InlineData("A58A18501")]
    [InlineData("A58-18501")]
    public void IsValidIdentification_InvalidCif_ReturnsFalse(string cif)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(cif, SpanishIdentificationType.CIF);

        Assert.False(result);
    }
    #endregion

    #region Formato correcto pero tipo de identificación incorrecto
    [Theory]
    // Formato DNI pero se indica NIE
    [InlineData("12345678Z", SpanishIdentificationType.NIE)]
    // Formato NIE pero se indica DNI
    [InlineData("X1234567L", SpanishIdentificationType.DNI)]
    // Formato CIF pero se indica DNI
    [InlineData("A58818501", SpanishIdentificationType.DNI)]
    // Formato CIF pero se indica NIE
    [InlineData("A58818501", SpanishIdentificationType.NIE)]
    public void IsValidIdentification_FormatDoesNotMatchType_ReturnsFalse(string id, SpanishIdentificationType type)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(id, type);

        Assert.False(result);
    }
    #endregion
}