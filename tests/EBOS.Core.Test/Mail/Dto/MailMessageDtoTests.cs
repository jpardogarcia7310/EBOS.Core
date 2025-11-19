using EBOS.Core.Mail.Dto;
using System.Collections.ObjectModel;

namespace EBOS.Core.Test.Mail.Dto;

public class MailMessageDtoTests
{
    [Fact]
    public void DefaultValues_AreInitialized()
    {
        // Act
        var dto = new MailMessageDto();

        // Assert
        Assert.NotNull(dto.FromAddress);
        Assert.IsType<Collection<MailAddressDto>>(dto.FromAddress);
        Assert.Empty(dto.FromAddress);
        Assert.NotNull(dto.ToAddress);
        Assert.IsType<Collection<MailAddressDto>>(dto.ToAddress);
        Assert.Empty(dto.ToAddress);
        Assert.Equal(string.Empty, dto.Subject);
        Assert.Equal(string.Empty, dto.Message);
        Assert.Equal(string.Empty, dto.BodyType);
        Assert.Null(dto.MailAttachment);
    }

    [Fact]
    public void CanAddAddressesToCollections()
    {
        // Arrange
        var dto = new MailMessageDto();
        var from = new MailAddressDto("Desde", "desde@ejemplo.com");
        var to = new MailAddressDto("Hasta", "hasta@ejemplo.com");

        // Act
        dto.FromAddress.Add(from);
        dto.ToAddress.Add(to);

        // Assert
        Assert.Single(dto.FromAddress);
        Assert.Same(from, dto.FromAddress[0]);
        Assert.Single(dto.ToAddress);
        Assert.Same(to, dto.ToAddress[0]);
    }

    [Fact]
    public void CanSetBasicProperties_AndAttachment()
    {
        // Arrange
        var dto = new MailMessageDto();
        var attachment = new MailAttachmentDto
        {
            MediaType = "image/png",
            FileName = "imagen.png",
            Content = [10, 20]
        };

        // Act
        dto.Subject = "Asunto";
        dto.Message = "Cuerpo del mensaje";
        dto.BodyType = "text/plain";
        dto.MailAttachment = attachment;

        // Assert
        Assert.Equal("Asunto", dto.Subject);
        Assert.Equal("Cuerpo del mensaje", dto.Message);
        Assert.Equal("text/plain", dto.BodyType);
        Assert.Same(attachment, dto.MailAttachment);
    }
}