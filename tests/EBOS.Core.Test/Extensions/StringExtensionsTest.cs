using EBOS.Core.Extensions;

namespace EBOS.Core.Test.Extensions;

public class StringExtensionsTests
{
    #region ToDateFromStringIso8601Format
    [Fact]
    public void ToDateFromStringIso8601Format_NullString_ReturnsNull()
    {
        string? input = null;
        var result = input!.ToDateFromStringIso8601Format();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringIso8601Format_EmptyString_ReturnsNull()
    {
        var input = string.Empty;
        var result = input.ToDateFromStringIso8601Format();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringIso8601Format_WhitespaceString_ReturnsNull()
    {
        var input = "   ";
        var result = input.ToDateFromStringIso8601Format();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringIso8601Format_ValidDate_ReturnsExpectedDate()
    {
        var input = "20251119";
        var result = input.ToDateFromStringIso8601Format();

        Assert.NotNull(result);
        Assert.Equal(
            new DateTime(2025, 11, 19, 0, 0, 0, DateTimeKind.Unspecified),
            result.Value
        );
    }

    [Fact]
    public void ToDateFromStringIso8601Format_InvalidFormat_ReturnsNull()
    {
        var input = "19/11/2025";
        var result = input.ToDateFromStringIso8601Format();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringIso8601Format_InvalidDate_ReturnsNull()
    {
        var input = "20251340"; // invalid month and day
        var result = input.ToDateFromStringIso8601Format();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringIso8601Format_WrongLength_ReturnsNull()
    {
        var input = "202511"; // too short
        var result = input.ToDateFromStringIso8601Format();

        Assert.Null(result);
    }
    #endregion

    #region ToDateFromStringWithHHMM
    [Fact]
    public void ToDateFromStringWithHHMM_NullString_ReturnsNull()
    {
        string? input = null;
        var result = input!.ToDateFromStringWithHHMM();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringWithHHMM_EmptyString_ReturnsNull()
    {
        var input = string.Empty;
        var result = input.ToDateFromStringWithHHMM();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringWithHHMM_WhitespaceString_ReturnsNull()
    {
        var input = "   ";
        var result = input.ToDateFromStringWithHHMM();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringWithHHMM_ValidDateAndTime_ReturnsExpectedDateTime()
    {
        var input = "202511191345";
        var result = input.ToDateFromStringWithHHMM();

        Assert.NotNull(result);
        Assert.Equal(
            new DateTime(2025, 11, 19, 13, 45, 0, DateTimeKind.Unspecified),
            result.Value
        );
    }

    [Fact]
    public void ToDateFromStringWithHHMM_InvalidFormat_ReturnsNull()
    {
        var input = "2025-11-19 13:45";
        var result = input.ToDateFromStringWithHHMM();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringWithHHMM_InvalidTime_ReturnsNull()
    {
        var input = "202511192565"; // invalid 25h 65m
        var result = input.ToDateFromStringWithHHMM();

        Assert.Null(result);
    }

    [Fact]
    public void ToDateFromStringWithHHMM_WrongLength_ReturnsNull()
    {
        var input = "20251119"; // missing HHmm
        var result = input.ToDateFromStringWithHHMM();

        Assert.Null(result);
    }
    #endregion

    #region RemoveSpecialCharacters
    [Fact]
    public void RemoveSpecialCharacters_NullString_ReturnsEmptyString()
    {
        string? input = null;
        var result = input!.RemoveSpecialCharacters();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void RemoveSpecialCharacters_EmptyString_ReturnsEmptyString()
    {
        var input = string.Empty;
        var result = input.RemoveSpecialCharacters();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void RemoveSpecialCharacters_WhitespaceOnly_ReturnsEmptyString()
    {
        var input = "   \t  ";
        var result = input.RemoveSpecialCharacters();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void RemoveSpecialCharacters_TextWithoutSpecialCharacters_ReturnsSameText()
    {
        var input = "Texto normal 123 ABC";
        var result = input.RemoveSpecialCharacters();

        Assert.Equal(input, result);
    }

    [Fact]
    public void RemoveSpecialCharacters_RemovesNewlinesTabsAndQuotesAndOthers()
    {
        var input = "Hola,\n\"mundo\"\r\n\t& 'texto', fin" + Environment.NewLine;
        var result = input.RemoveSpecialCharacters();

        // After removing newlines, tabs, quotes, commas, apostrophes, and &
        // the result is: "Holamundo texto fin"
        Assert.Equal("Holamundo texto fin", result);
    }

    [Fact]
    public void RemoveSpecialCharacters_RemovesMultipleOccurrencesOfSpecialCharacters()
    {
        var input = "\n\nTexto\r\rcon\t\tcaracteres\"\"especiales,,''&&" + Environment.NewLine;
        var result = input.RemoveSpecialCharacters();

        Assert.Equal("Textoconcaracteresespeciales", result);
    }

    [Fact]
    public void RemoveSpecialCharacters_DoesNotRemoveNormalSpacesInsideText()
    {
        var input = "Hola \n mundo \t bonito & feliz";
        var result = input.RemoveSpecialCharacters();

        // Special chars are removed, but normal spaces are preserved
        Assert.Equal("Hola  mundo  bonito  feliz", result);
    }
    #endregion
}
