using EBOS.Core.Enums;
using EBOS.Core.Validators;

namespace EBOS.Core.Test.Validators;

public class SpanishIdentificationValidatorTests
{
    [Theory]
    [InlineData("00000000T", true)]
    public void ValidateDni_ValidExamples(string dni, bool expected)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(dni, SpanishIdentificationType.DNI);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("12345678A", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void ValidateDni_InvalidExamples(string? dni, bool expected)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(dni ?? string.Empty, SpanishIdentificationType.DNI);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("X0000000T", true)]
    public void ValidateNie_ValidExamples(string nie, bool expected)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(nie, SpanishIdentificationType.NIE);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("A1234567L", false)]
    [InlineData("X1234", false)]
    public void ValidateNie_InvalidExamples(string nie, bool expected)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(nie, SpanishIdentificationType.NIE);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("A58818501", true)]
    public void ValidateCif_ValidExamples(string cif, bool expected)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(cif, SpanishIdentificationType.CIF);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("A5881850", false)]
    [InlineData("123", false)]
    public void ValidateCif_InvalidExamples(string cif, bool expected)
    {
        var result = SpanishIdentificationValidator.IsValidIdentification(cif, SpanishIdentificationType.CIF);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Validate_UnknownType_ReturnsFalse()
    {
        var result = SpanishIdentificationValidator.IsValidIdentification("00000000T", (SpanishIdentificationType)999);
        Assert.False(result);
    }
}
