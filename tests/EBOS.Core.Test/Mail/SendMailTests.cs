using EBOS.Core.Mail;
using EBOS.Core.Mail.Dto;
using MimeKit;
using System.Reflection;

namespace EBOS.Core.Test.Mail;

public class SendMailTests
{
    // Helpers para crear DTOs válidos de base

    private static MailMessageDto CreateBaseMessage()
    {
        var dto = new MailMessageDto
        {
            Subject = "Asunto",
            Message = "Cuerpo",
            BodyType = "plain" // o el valor que uses en tu código
        };

        dto.FromAddress.Add(new MailAddressDto("Remitente", "remitente@dominio.com"));
        dto.ToAddress.Add(new MailAddressDto("Destinatario", "destinatario@dominio.com"));

        return dto;
    }

    private static MailSettingsDto CreateBaseSettings(bool sendMail = true, bool withUser = true)
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

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task SendAsync_EmptyOrWhitespaceSubject_ThrowsInvalidOperationException(string subject)
    {
        // Arrange
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = new MailMessageDto
        {
            Subject = subject,
            Message = "Cuerpo",
            BodyType = "plain"
        };

        messageDto.FromAddress.Add(new MailAddressDto("Remitente", "remitente@dominio.com"));
        messageDto.ToAddress.Add(new MailAddressDto("Destinatario", "destinatario@dominio.com"));

        var settings = new MailSettingsDto
        {
            Server = "smtp.servidor.com",
            Port = 587,
            HasSSL = true,
            SendMail = true,
            MailUser = "user@dominio.com",
            MailPassword = "pwd"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.SendAsync(messageDto, settings));
    }

    [Fact]
    public void Ctor_NullSmtpClientFactory_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SendMail(null!));
    }

    [Fact]
    public async Task SendAsync_WithValidAttachment_SendsMultipartMixedWithAttachment()
    {
        // Arrange
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = new MailMessageDto
        {
            Subject = "Asunto con adjunto",
            Message = "Mensaje con adjunto",
            BodyType = "plain"
        };

        messageDto.FromAddress.Add(new MailAddressDto("Remitente", "remitente@dominio.com"));
        messageDto.ToAddress.Add(new MailAddressDto("Destinatario", "destinatario@dominio.com"));
        messageDto.MailAttachment = new MailAttachmentDto
        {
            MediaType = "application/pdf",
            FileName = "test.pdf",
            Content = [1, 2, 3]
        };
        var settings = new MailSettingsDto
        {
            Server = "smtp.servidor.com",
            Port = 587,
            HasSSL = true,
            SendMail = true,
            MailUser = "user@dominio.com",
            MailPassword = "pwd"
        };

        // Act
        await sut.SendAsync(messageDto, settings);

        // Assert
        var client = factory.Instance;

        Assert.True(client.ConnectCalled);
        Assert.True(client.SendCalled);
        Assert.True(client.DisconnectCalled);

        Assert.NotNull(client.LastMessage);
        var mimeMessage = client.LastMessage!;

        Assert.Equal("Asunto con adjunto", mimeMessage.Subject);

        var multipart = Assert.IsType<Multipart>(mimeMessage.Body);
        Assert.Equal("mixed", multipart.ContentType.MediaSubtype);

        // Debe haber 2 partes: cuerpo + adjunto
        Assert.Equal(2, multipart.Count);

        var bodyPart = Assert.IsType<TextPart>(multipart[0]);
        Assert.Equal("Mensaje con adjunto", bodyPart.Text);

        var attachmentPart = Assert.IsType<MimePart>(multipart[1]);
        Assert.Equal("test.pdf", attachmentPart.FileName);
        Assert.Equal("application", attachmentPart.ContentType.MediaType);
        Assert.Equal("pdf", attachmentPart.ContentType.MediaSubtype);
    }

    // -------------------------- Parámetros nulos --------------------------

    [Fact]
    public async Task SendAsync_NullMailMessage_ThrowsArgumentNullException()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var settings = CreateBaseSettings();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.SendAsync(null!, settings));
    }

    [Fact]
    public async Task SendAsync_NullMailSettings_ThrowsArgumentNullException()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var message = CreateBaseMessage();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.SendAsync(message, null!));
    }

    // -------------------------- SendMail = false --------------------------

    [Fact]
    public async Task SendAsync_SendMailFalse_DoesNotCallSmtpClient()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings(sendMail: false);

        await sut.SendAsync(messageDto, settings);

        var client = factory.Instance;

        Assert.False(client.ConnectCalled);
        Assert.False(client.AuthenticateCalled);
        Assert.False(client.SendCalled);
        Assert.False(client.DisconnectCalled);
        Assert.False(client.Disposed);
    }

    // -------------------------- Validación From / To --------------------------

    [Fact]
    public async Task SendAsync_EmptyFromAddress_ThrowsInvalidOperationException()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings();

        messageDto.FromAddress.Clear(); // sin remitentes

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.SendAsync(messageDto, settings));
    }

    [Fact]
    public async Task SendAsync_EmptyToAddress_ThrowsInvalidOperationException()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings();

        messageDto.ToAddress.Clear(); // sin destinatarios

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.SendAsync(messageDto, settings));
    }

    // -------------------------- BodyType por defecto --------------------------

    [Fact]
    public async Task SendAsync_EmptyBodyType_DefaultsToPlainText()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings();

        messageDto.BodyType = string.Empty; // forzamos vacío

        await sut.SendAsync(messageDto, settings);

        var client = factory.Instance;
        Assert.True(client.SendCalled);
        Assert.NotNull(client.LastMessage);

        var body = Assert.IsType<TextPart>(client.LastMessage!.Body);

        // MimeKit: TextPart("plain") -> ContentType.MimeType == "text/plain"
        Assert.Equal("text/plain", body.ContentType.MimeType);
    }

    // -------------------------- Validación de adjunto --------------------------

    [Fact]
    public async Task SendAsync_AttachmentWithNullContent_ThrowsInvalidOperationException()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();

        messageDto.MailAttachment = new MailAttachmentDto
        {
            MediaType = "application/pdf",
            FileName = "test.pdf",
            Content = null! // null intencionado
        };

        var settings = CreateBaseSettings();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.SendAsync(messageDto, settings));
    }

    [Fact]
    public async Task SendAsync_AttachmentWithEmptyContent_ThrowsInvalidOperationException()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();

        messageDto.MailAttachment = new MailAttachmentDto
        {
            MediaType = "application/pdf",
            FileName = "test.pdf",
            Content = []
        };

        var settings = CreateBaseSettings();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.SendAsync(messageDto, settings));
    }

    [Fact]
    public async Task SendAsync_AttachmentWithoutMediaType_ThrowsInvalidOperationException()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();

        messageDto.MailAttachment = new MailAttachmentDto
        {
            MediaType = string.Empty,
            FileName = "test.pdf",
            Content = [1, 2, 3]
        };

        var settings = CreateBaseSettings();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.SendAsync(messageDto, settings));
    }

    [Fact]
    public async Task SendAsync_AttachmentWithoutFileName_ThrowsInvalidOperationException()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();

        messageDto.MailAttachment = new MailAttachmentDto
        {
            MediaType = "application/pdf",
            FileName = "",
            Content = [1, 2, 3]
        };

        var settings = CreateBaseSettings();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.SendAsync(messageDto, settings));
    }

    // -------------------------- Flujo SMTP completo --------------------------

    [Fact]
    public async Task SendAsync_ValidMessageAndSettings_CallsAllSmtpOperations()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings(sendMail: true, withUser: true);

        await sut.SendAsync(messageDto, settings);

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
    public async Task SendAsync_WithoutUser_DoesNotAuthenticate()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings(sendMail: true, withUser: false);

        await sut.SendAsync(messageDto, settings);

        var client = factory.Instance;

        Assert.True(client.ConnectCalled);
        Assert.False(client.AuthenticateCalled);
        Assert.True(client.SendCalled);
        Assert.True(client.DisconnectCalled);
    }

    // -------------------------- Mapeo MimeMessage (sin adjunto / con adjunto) --------------------------

    [Fact]
    public void CreateMessage_WithoutAttachment_MapsFieldsCorrectly()
    {
        var messageDto = CreateBaseMessage();
        messageDto.BodyType = "plain";
        // método privado estático, usamos reflexión
        var method = typeof(SendMail).GetMethod(
            "CreateMessage",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        using var mimeMessage = (MimeMessage)method!.Invoke(null, [messageDto])!;

        // From
        Assert.Single(mimeMessage.From);
        var from = (MailboxAddress)mimeMessage.From[0];
        Assert.Equal("Remitente", from.Name);
        Assert.Equal("remitente@dominio.com", from.Address);

        // To
        Assert.Single(mimeMessage.To);
        var to = (MailboxAddress)mimeMessage.To[0];
        Assert.Equal("Destinatario", to.Name);
        Assert.Equal("destinatario@dominio.com", to.Address);

        // Subject
        Assert.Equal("Asunto", mimeMessage.Subject);

        // Body
        var body = Assert.IsType<TextPart>(mimeMessage.Body);
        Assert.Equal("Cuerpo", body.Text);
    }

    [Fact]
    public void CreateMessage_WithAttachment_CreatesMultipartMixedWithBodyAndAttachment()
    {
        var messageDto = CreateBaseMessage();

        messageDto.Subject = "Asunto con adjunto";
        messageDto.Message = "Mensaje con adjunto";
        messageDto.BodyType = "plain";
        messageDto.MailAttachment = new MailAttachmentDto
        {
            MediaType = "application/pdf",
            FileName = "test.pdf",
            Content = [1, 2, 3]
        };
        var method = typeof(SendMail).GetMethod(
            "CreateMessage",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        using var mimeMessage = (MimeMessage)method!.Invoke(null, [messageDto])!;

        Assert.Equal("Asunto con adjunto", mimeMessage.Subject);

        var multipart = Assert.IsType<Multipart>(mimeMessage.Body);
        Assert.Equal("mixed", multipart.ContentType.MediaSubtype);
        Assert.Equal(2, multipart.Count);

        var bodyPart = Assert.IsType<TextPart>(multipart[0]);
        Assert.Equal("Mensaje con adjunto", bodyPart.Text);

        var attachmentPart = Assert.IsType<MimePart>(multipart[1]);
        Assert.Equal("test.pdf", attachmentPart.FileName);
    }

    // -------------------------- Wiring del constructor por defecto --------------------------

    [Fact]
    public void DefaultConstructor_UsesMailKitSmtpClientFactory()
    {
        var sut = new SendMail();
        var field = typeof(SendMail).GetField("_smtpClientFactory",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(field);

        var value = field!.GetValue(sut);

        Assert.NotNull(value);
        Assert.IsType<MailKitSmtpClientFactory>(value);
    }
}