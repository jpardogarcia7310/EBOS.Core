using EBOS.Core.Mail;
using EBOS.Core.Mail.Dto;

namespace EBOS.Core.Test.Mail;

public class SendMail_SendFlowTests
{
    private static MailMessageDto CreateSampleMessage()
    {
        var dto = new MailMessageDto
        {
            Subject = "Asunto",
            Message = "Cuerpo",
            BodyType = "text/plain"
        };
        dto.FromAddress.Add(new MailAddressDto("Remitente", "remitente@dominio.com"));
        dto.ToAddress.Add(new MailAddressDto("Destinatario", "destinatario@dominio.com"));
        return dto;
    }

    private static MailSettingsDto CreateSampleSettings(bool sendMail = true, bool withUser = true)
    {
        return new MailSettingsDto
        {
            Server = "smtp.servidor.com",
            Port = 587,
            HasSSL = true,
            SendMail = sendMail,
            MailUser = withUser ? "user@dominio.com" : string.Empty,
            MailPassword = withUser ? "pwd" : string.Empty
        };
    }

    [Fact]
    public async Task SendAsync_SendMailTrue_CallsAllSmtpOperations()
    {
        // Arrange
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateSampleMessage();
        var settings = CreateSampleSettings(sendMail: true, withUser: true);

        // Act
        await sut.SendAsync(messageDto, settings);

        // Assert
        var client = factory.Instance;

        Assert.True(client.ConnectCalled);
        Assert.True(client.AuthenticateCalled);
        Assert.True(client.SendCalled);
        Assert.True(client.DisconnectCalled);
        Assert.True(client.Disposed);
        Assert.Equal(settings.Server, client.Host);
        Assert.Equal(settings.Port, client.Port);
        Assert.Equal(settings.HasSSL, client.UseSsl);
        Assert.Equal(settings.MailUser, client.UserName);
        Assert.Equal(settings.MailPassword, client.Password);
        Assert.NotNull(client.LastMessage);
        Assert.Equal(messageDto.Subject, client.LastMessage!.Subject);
    }

    [Fact]
    public async Task SendAsync_SendMailFalse_DoesNotCallSmtpClient()
    {
        // Arrange
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateSampleMessage();
        var settings = CreateSampleSettings(sendMail: false);

        // Act
        await sut.SendAsync(messageDto, settings);

        // Assert
        var client = factory.Instance;

        Assert.False(client.ConnectCalled);
        Assert.False(client.AuthenticateCalled);
        Assert.False(client.SendCalled);
        Assert.False(client.DisconnectCalled);
        // Puede estar a true o false según si el using llegó a ejecutarse (aquí no se crea cliente)
        Assert.False(client.Disposed);
    }

    [Fact]
    public async Task SendAsync_WithoutUser_DoesNotAuthenticate()
    {
        // Arrange
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateSampleMessage();
        var settings = CreateSampleSettings(sendMail: true, withUser: false);

        // Act
        await sut.SendAsync(messageDto, settings);

        // Assert
        var client = factory.Instance;

        Assert.True(client.ConnectCalled);
        Assert.False(client.AuthenticateCalled);
        Assert.True(client.SendCalled);
        Assert.True(client.DisconnectCalled);
    }
}