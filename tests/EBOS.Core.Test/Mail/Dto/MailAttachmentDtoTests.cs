using EBOS.Core.Mail.Dto;

namespace EBOS.Core.Test.Mail.Dto;

public class MailAttachmentDtoTests
{
    [Fact]
    public void DefaultValues_AreInitialized()
    {
        // Act
        var dto = new MailAttachmentDto();

        // Assert
        Assert.NotNull(dto.MediaType);
        Assert.Equal(string.Empty, dto.MediaType);
        Assert.NotNull(dto.FileName);
        Assert.Equal(string.Empty, dto.FileName);
        Assert.NotNull(dto.Content);
        Assert.Empty(dto.Content);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var dto = new MailAttachmentDto();
        var content = new byte[] { 1, 2, 3 };

        // Act
        dto.MediaType = "application/pdf";
        dto.FileName = "fichero.pdf";
        dto.Content = content;

        // Assert
        Assert.Equal("application/pdf", dto.MediaType);
        Assert.Equal("fichero.pdf", dto.FileName);
        Assert.Same(content, dto.Content);
    }
}