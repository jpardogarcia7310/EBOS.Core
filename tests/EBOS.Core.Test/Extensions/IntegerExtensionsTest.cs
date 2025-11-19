using EBOS.Core.Extensions;
using System.Globalization;

namespace EBOS.Core.Test.Extensions;

public class IntegerExtensionsTests
{
    #region Valid numeric strings
    [Fact]
    public void ParseOrDefault_ValidPositiveInteger_ReturnsParsedValue()
    {
        var input = "123";
        int? defaultValue = null;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(123, result);
    }

    [Fact]
    public void ParseOrDefault_ValidNegativeInteger_ReturnsParsedValue()
    {
        var input = "-456";
        int? defaultValue = null;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(-456, result);
    }

    [Fact]
    public void ParseOrDefault_Zero_ReturnsZero()
    {
        var input = "0";
        int? defaultValue = 999;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(0, result);
    }

    [Fact]
    public void ParseOrDefault_IntMaxValue_ReturnsMaxValue()
    {
        var input = int.MaxValue.ToString(CultureInfo.InvariantCulture);
        int? defaultValue = null;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(int.MaxValue, result);
    }

    [Fact]
    public void ParseOrDefault_IntMinValue_ReturnsMinValue()
    {
        var input = int.MinValue.ToString(CultureInfo.InvariantCulture);
        int? defaultValue = null;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(int.MinValue, result);
    }
    #endregion

    #region Invalid format -> defaultValue behavior
    [Fact]
    public void ParseOrDefault_NonNumericStringWithDefaultValue_ReturnsDefaultValue()
    {
        var input = "abc";
        int? defaultValue = 42;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseOrDefault_NonNumericStringWithNullDefault_ReturnsNull()
    {
        var input = "not-an-int";
        int? defaultValue = null;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Null(result);
    }

    [Fact]
    public void ParseOrDefault_EmptyStringWithDefaultValue_ReturnsDefaultValue()
    {
        var input = string.Empty;
        int? defaultValue = -1;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseOrDefault_WhitespaceStringWithDefaultValue_ReturnsDefaultValue()
    {
        var input = "   ";
        int? defaultValue = 7;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseOrDefault_StringWithDecimalPoint_ReturnsDefaultValue()
    {
        var input = "123.45"; // inválido para int.Parse con InvariantCulture
        int? defaultValue = 100;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseOrDefault_StringWithThousandsSeparatorInvariant_ReturnsDefaultValue()
    {
        var input = "1,234"; // con InvariantCulture esto no es válido para int.Parse simple
        int? defaultValue = 500;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(defaultValue, result);
    }
    #endregion

    #region Overflow / Underflow
    [Fact]
    public void ParseOrDefault_ValueGreaterThanIntMax_ReturnsDefaultValue()
    {
        // int.MaxValue + "0" garantiza overflow
        var input = ((long)int.MaxValue + 1).ToString(CultureInfo.InvariantCulture);
        int? defaultValue = 123;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseOrDefault_ValueLessThanIntMin_ReturnsDefaultValue()
    {
        var input = ((long)int.MinValue - 1).ToString(CultureInfo.InvariantCulture);
        int? defaultValue = -999;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseOrDefault_OverflowWithNullDefault_ReturnsNull()
    {
        var input = ((long)int.MaxValue + 1).ToString(CultureInfo.InvariantCulture);
        int? defaultValue = null;
        var result = IntegerExtensions.ParseOrDefault(input, defaultValue);

        Assert.Null(result);
    }
    #endregion

    #region Null input
    [Fact]
    public void ParseOrDefault_NullInput_ThrowsArgumentNullException()
    {
        string? input = null;
        int? defaultValue = 10;

        Assert.Throws<ArgumentNullException>(() =>
            IntegerExtensions.ParseOrDefault(input!, defaultValue));
    }
    #endregion
}
