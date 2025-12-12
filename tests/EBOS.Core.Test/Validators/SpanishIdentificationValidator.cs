using EBOS.Core.Enums;
using EBOS.Core.Validators;

namespace EBOS.Core.Test.Validators;

public class SpanishIdentificationValidatorTests
{
    [Theory]
    [InlineData("12345678Z", true)] // example valid DNI (Z depends on number; replace with a valid pair)
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
    // NIE examples: X1234567L (replace with valid combos if needed)
    [InlineData("X1234567L", true)]
    [InlineData("Y1234567X", true)]
    [InlineData("Z1234567R", true)]
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
    // CIF: use a few representative patterns. These examples are structural; for production tests use official valid CIFs.
    [InlineData("A58818501", true)] // example company CIF (replace with known valid if needed)
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
}