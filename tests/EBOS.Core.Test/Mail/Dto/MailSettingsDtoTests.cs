using EBOS.Core.Mail.Dto;

namespace EBOS.Core.Test.Mail.Dto;

public class MailSettingsDtoTests
{
    [Fact]
    public void DefaultValues_AreInitialized()
    {
        // Act
        var dto = new MailSettingsDto();

        // Assert
        Assert.NotNull(dto.Server);
        Assert.Equal(string.Empty, dto.Server);
        Assert.NotNull(dto.MailUser);
        Assert.Equal(string.Empty, dto.MailUser);
        Assert.NotNull(dto.MailPassword);
        Assert.Equal(string.Empty, dto.MailPassword);
        Assert.Equal(0, dto.Port);
        Assert.False(dto.HasSSL);
        Assert.False(dto.SendMail);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var dto = new MailSettingsDto
        {
            // Act
            Server = "smtp.servidor.com",
            Port = 587,
            MailUser = "usuario@servidor.com",
            MailPassword = "password",
            HasSSL = true,
            SendMail = true
        };

        // Assert
        Assert.Equal("smtp.servidor.com", dto.Server);
        Assert.Equal(587, dto.Port);
        Assert.Equal("usuario@servidor.com", dto.MailUser);
        Assert.Equal("password", dto.MailPassword);
        Assert.True(dto.HasSSL);
        Assert.True(dto.SendMail);
    }
}