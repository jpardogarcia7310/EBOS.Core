using EBOS.Core.Extensions;
using System.Globalization;

namespace EBOS.Core.Test.Extensions;

public class DateExtensionsTests
{
    #region ToDateFormat (DateTime?)
    [Fact]
    public void ToDateFormat_NullDate_ReturnsEmptyString()
    {
        DateTime? date = null;
        var result = date.ToDateFormat();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ToDateFormat_ValidDate_FormatsAsyyyyMMdd()
    {
        DateTime? date = new DateTime(2025, 11, 19, 0, 0, 0, DateTimeKind.Unspecified);
        var result = date.ToDateFormat();

        Assert.Equal("20251119", result);
    }

    #endregion

    #region ToDateFormatWithTime (DateTime?)

    [Fact]
    public void ToDateFormatWithTime_NullNullableDate_ReturnsEmptyString()
    {
        DateTime? date = null;
        var result = date.ToDateFormatWithTime();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ToDateFormatWithTime_ValidNullableDate_FormatsAsyyyyMMddHHmmss()
    {
        DateTime? date = new DateTime(2025, 11, 19, 13, 45, 59, DateTimeKind.Unspecified);
        var result = date.ToDateFormatWithTime();

        Assert.Equal("20251119134559", result);
    }

    #endregion

    #region ToDateFormatWithTime (DateTime)

    [Fact]
    public void ToDateFormatWithTime_NonNullableDate_FormatsAsyyyyMMddHHmmss()
    {
        var date = new DateTime(2025, 01, 02, 03, 04, 05, DateTimeKind.Unspecified);
        var result = date.ToDateFormatWithTime();

        Assert.Equal("20250102030405", result);
    }
    #endregion

    #region ToDateFormatWithTimeISO8601 (DateTime)
    [Fact]
    public void ToDateFormatWithTimeISO8601_ValidDate_FormatsAsISO8601WithZ()
    {
        var date = new DateTime(2025, 12, 31, 23, 59, 58, DateTimeKind.Unspecified);
        var result = date.ToDateFormatWithTimeISO8601();

        Assert.Equal("2025-12-31T23:59:58Z", result);
    }
    #endregion

    #region ParseExactOrDefault
    [Fact]
    public void ParseExactOrDefault_ValidInput_ReturnsParsedDate()
    {
        var input = "20251119";
        var format = "yyyyMMdd";
        DateTime? defaultValue = null;
        var result = DateExtensions.ParseExactOrDefault(input, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.True(result.HasValue);
        Assert.Equal(new DateTime(2025, 11, 19, 0, 0, 0, DateTimeKind.Unspecified), result.Value);
    }

    [Fact]
    public void ParseExactOrDefault_InvalidFormatInInput_ReturnsDefaultValue()
    {
        var input = "19-11-2025";
        var format = "yyyyMMdd";
        DateTime? defaultValue = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var result = DateExtensions.ParseExactOrDefault(input, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseExactOrDefault_NullInput_ReturnsDefaultValue()
    {
        string? input = null;
        var format = "yyyyMMdd";
        DateTime? defaultValue = new DateTime(1999, 12, 31, 0, 0, 0, DateTimeKind.Unspecified);
        var result = DateExtensions.ParseExactOrDefault(input ?? string.Empty, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseExactOrDefault_EmptyFormat_ThrowsArgumentExceptionAndReturnsDefault()
    {
        var input = "20251119";
        string format = string.Empty;
        DateTime? defaultValue = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var result = DateExtensions.ParseExactOrDefault(input, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParseExactOrDefault_NullFormat_ReturnsDefaultValue()
    {
        var input = "20251119";
        string? format = null;
        DateTime? defaultValue = new DateTime(2015, 5, 5, 0, 0, 0, DateTimeKind.Unspecified);
        var result = DateExtensions.ParseExactOrDefault(input, format ?? string.Empty, CultureInfo.InvariantCulture, defaultValue);

        Assert.Equal(defaultValue, result);
    }
    #endregion

    #region ParsePart
    [Fact]
    public void ParsePart_ValidMatchAndValidDate_ReturnsParsedDate()
    {
        var input = "The date is 20251119 inside text.";
        var regex = @"\d{8}";
        var format = "yyyyMMdd";
        DateTime? defaultValue = null;
        var result = DateExtensions.ParsePart(input, regex, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Equal(new DateTime(2025, 11, 19, 0, 0, 0, DateTimeKind.Unspecified), result);
    }

    [Fact]
    public void ParsePart_ValidMatchButInvalidDate_ReturnsDefaultValue()
    {
        var input = "The date is 20251340 inside text."; // invalid month and day
        var regex = @"\d{8}";
        var format = "yyyyMMdd";
        DateTime? defaultValue = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var result = DateExtensions.ParsePart(input, regex, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParsePart_ValidMatchButInvalidDate_DefaultValueNull_ReturnsNull()
    {
        var input = "The date is 20251340 inside text."; // invalid
        var regex = @"\d{8}";
        var format = "yyyyMMdd";
        DateTime? defaultValue = null;
        var result = DateExtensions.ParsePart(input, regex, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Null(result);
    }

    [Fact]
    public void ParsePart_NoMatch_ReturnsDefaultValue()
    {
        var input = "There is no date here.";
        var regex = @"\d{8}";
        var format = "yyyyMMdd";
        DateTime? defaultValue = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var result = DateExtensions.ParsePart(input, regex, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParsePart_NoMatch_DefaultValueNull_ReturnsNull()
    {
        var input = "There is no date here.";
        var regex = @"\d{8}";
        var format = "yyyyMMdd";
        DateTime? defaultValue = null;
        var result = DateExtensions.ParsePart(input, regex, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Null(result);
    }

    [Fact]
    public void ParsePart_InvalidRegex_ReturnsDefaultValue()
    {
        var input = "20251119";
        var regex = "["; // invalid regex => ArgumentException
        var format = "yyyyMMdd";
        DateTime? defaultValue = new DateTime(2011, 11, 11, 0, 0, 0, DateTimeKind.Unspecified);
        var result = DateExtensions.ParsePart(input, regex, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Equal(defaultValue, result);
    }

    [Fact]
    public void ParsePart_InvalidRegex_DefaultValueNull_ReturnsNull()
    {
        var input = "20251119";
        var regex = "["; // invalid regex
        var format = "yyyyMMdd";
        DateTime? defaultValue = null;
        var result = DateExtensions.ParsePart(input, regex, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Null(result);
    }

    [Fact]
    public void ParsePart_NullInput_ReturnsDefaultValue()
    {
        string? input = null;
        var regex = @"\d{8}";
        var format = "yyyyMMdd";
        DateTime? defaultValue = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var result = DateExtensions.ParsePart(input!, regex, format, CultureInfo.InvariantCulture, defaultValue);

        Assert.Equal(defaultValue, result);
    }
    #endregion
}