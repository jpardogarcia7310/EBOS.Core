using EBOS.Core.Mail;
using EBOS.Core.Mail.Dto;
using MimeKit;
using System.Reflection;

namespace EBOS.Core.Test.Mail;

public class SendMailTests
{
    // Helpers to create valid DTOs.
    private sealed class ThrowingSmtpClientAdapter(Exception? onConnect = null, Exception? onAuthenticate = null, Exception? onSend = null, Exception? onDisconnect = null) : ISmtpClientAdapter
    {
        private readonly Exception? _onConnect = onConnect;
        private readonly Exception? _onAuthenticate = onAuthenticate;
        private readonly Exception? _onSend = onSend;
        private readonly Exception? _onDisconnect = onDisconnect;

        public Task ConnectAsync(string host, int port, bool useSsl, CancellationToken cancellationToken = default)
        {
            if (_onConnect is not null) throw _onConnect;
            return Task.CompletedTask;
        }

        public Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default)
        {
            if (_onAuthenticate is not null) throw _onAuthenticate;
            return Task.CompletedTask;
        }

        public Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default)
        {
            if (_onSend is not null) throw _onSend;
            return Task.CompletedTask;
        }

        public Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default)
        {
            if (_onDisconnect is not null) throw _onDisconnect;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }

    private sealed class ThrowingSmtpClientFactory(ISmtpClientAdapter adapter) : ISmtpClientFactory
    {
        private readonly ISmtpClientAdapter _adapter = adapter;
        public ISmtpClientAdapter Create() => _adapter;
        public ISmtpClientAdapter Create(SmtpClientOptions? options) => _adapter;
    }

    private sealed class TestProtocolException(string message) : MailKit.ProtocolException(message)
    {
    }

    private static MailKit.Net.Smtp.SmtpCommandException CreateSmtpCommandException()
    {
        var type = typeof(MailKit.Net.Smtp.SmtpCommandException);
        var ctor = type
            .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .OrderBy(c => c.GetParameters().Length)
            .First();

        var args = ctor
            .GetParameters()
            .Select(p => p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null)
            .ToArray();

        return (MailKit.Net.Smtp.SmtpCommandException)ctor.Invoke(args);
    }

    private static MailMessageDto CreateBaseMessage()
    {
        var dto = new MailMessageDto
        {
            Subject = "Subject",
            Message = "Body",
            BodyType = "plain" // or the value used in your code
        };

        dto.FromAddress.Add(new MailAddressDto("Sender", "sender@domain.com"));
        dto.ToAddress.Add(new MailAddressDto("Recipient", "recipient@domain.com"));

        return dto;
    }

    private static MailSettingsDto CreateBaseSettings(bool sendMail = true, bool withUser = true)
    {
        return new MailSettingsDto
        {
            Server = "smtp.server.com",
            Port = 587,
            HasSSL = true,
            SendMail = sendMail,
            MailUser = withUser ? "user@domain.com" : string.Empty,
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
            Message = "Body",
            BodyType = "plain"
        };

        messageDto.FromAddress.Add(new MailAddressDto("Sender", "sender@domain.com"));
        messageDto.ToAddress.Add(new MailAddressDto("Recipient", "recipient@domain.com"));

        var settings = new MailSettingsDto
        {
            Server = "smtp.server.com",
            Port = 587,
            HasSSL = true,
            SendMail = true,
            MailUser = "user@domain.com",
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
            Subject = "Subject with attachment",
            Message = "Message with attachment",
            BodyType = "plain"
        };

        messageDto.FromAddress.Add(new MailAddressDto("Sender", "sender@domain.com"));
        messageDto.ToAddress.Add(new MailAddressDto("Recipient", "recipient@domain.com"));
        messageDto.MailAttachment = new MailAttachmentDto
        {
            MediaType = "application/pdf",
            FileName = "test.pdf",
            Content = [1, 2, 3]
        };
        var settings = new MailSettingsDto
        {
            Server = "smtp.server.com",
            Port = 587,
            HasSSL = true,
            SendMail = true,
            MailUser = "user@domain.com",
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

        Assert.Equal("Subject with attachment", mimeMessage.Subject);

        var multipart = Assert.IsType<Multipart>(mimeMessage.Body);
        Assert.Equal("mixed", multipart.ContentType.MediaSubtype);

        // It should have 2 parts: body + attachment.
        Assert.Equal(2, multipart.Count);

        var bodyPart = Assert.IsType<TextPart>(multipart[0]);
        Assert.Equal("Message with attachment", bodyPart.Text);

        var attachmentPart = Assert.IsType<MimePart>(multipart[1]);
        Assert.Equal("test.pdf", attachmentPart.FileName);
        Assert.Equal("application", attachmentPart.ContentType.MediaType);
        Assert.Equal("pdf", attachmentPart.ContentType.MediaSubtype);
    }

    // -------------------------- Null parameters --------------------------

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

    // -------------------------- From / To validation --------------------------

    [Fact]
    public async Task SendAsync_EmptyFromAddress_ThrowsInvalidOperationException()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings();

        messageDto.FromAddress.Clear(); // no senders

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

        messageDto.ToAddress.Clear(); // no recipients

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.SendAsync(messageDto, settings));
    }

    // -------------------------- BodyType default --------------------------

    [Fact]
    public async Task SendAsync_EmptyBodyType_DefaultsToPlainText()
    {
        var factory = new FakeSmtpClientFactory();
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings();

        messageDto.BodyType = string.Empty; // force empty

        await sut.SendAsync(messageDto, settings);

        var client = factory.Instance;
        Assert.True(client.SendCalled);
        Assert.NotNull(client.LastMessage);

        var body = Assert.IsType<TextPart>(client.LastMessage!.Body);

        // MimeKit: TextPart("plain") -> ContentType.MimeType == "text/plain"
        Assert.Equal("text/plain", body.ContentType.MimeType);
    }

    // -------------------------- Attachment validation --------------------------

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
            Content = null! // intentional null
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

    // -------------------------- Full SMTP flow --------------------------

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

    // -------------------------- MimeMessage mapping (without / with attachment) --------------------------

    [Fact]
    public void CreateMessage_WithoutAttachment_MapsFieldsCorrectly()
    {
        var messageDto = CreateBaseMessage();
        messageDto.BodyType = "plain";

        var method = typeof(SendMail).GetMethod(
            "CreateMessage",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        // Invoke returns (MimeMessage, List<IDisposable>)
        var result = ((MimeMessage, List<IDisposable>))method!.Invoke(null, new object[] { messageDto })!;

        var mimeMessage = result.Item1;
        var disposables = result.Item2;

        try
        {
            // From
            Assert.Single(mimeMessage.From);
            var from = (MailboxAddress)mimeMessage.From[0];
            Assert.Equal("Sender", from.Name);
            Assert.Equal("sender@domain.com", from.Address);

            // To
            Assert.Single(mimeMessage.To);
            var to = (MailboxAddress)mimeMessage.To[0];
            Assert.Equal("Recipient", to.Name);
            Assert.Equal("recipient@domain.com", to.Address);

            // Subject
            Assert.Equal("Subject", mimeMessage.Subject);

            // Body
            var body = Assert.IsType<TextPart>(mimeMessage.Body);
            Assert.Equal("Body", body.Text);
        }
        finally
        {
            // Release helper resources returned by CreateMessage.
            if (disposables != null)
            {
                foreach (var d in disposables)
                {
                    try { d?.Dispose(); } catch { /* ignore disposal errors in tests */ }
                }
            }

            // Dispose MimeMessage if it implements IDisposable (MimeMessage does not implement it,
            // but if CreateMessage returns disposable objects, they are already released).
        }
    }

    [Fact]
    public void CreateMessage_WithAttachment_CreatesMultipartMixedWithBodyAndAttachment()
    {
        var messageDto = CreateBaseMessage();

        messageDto.Subject = "Subject with attachment";
        messageDto.Message = "Message with attachment";
        messageDto.BodyType = "plain";
        messageDto.MailAttachment = new MailAttachmentDto
        {
            MediaType = "application/pdf",
            FileName = "test.pdf",
            Content = new byte[] { 1, 2, 3 }
        };

        var method = typeof(SendMail).GetMethod(
            "CreateMessage",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        var result = ((MimeMessage, List<IDisposable>))method!.Invoke(null, new object[] { messageDto })!;

        var mimeMessage = result.Item1;
        var disposables = result.Item2;

        try
        {
            Assert.Equal("Subject with attachment", mimeMessage.Subject);

            var multipart = Assert.IsType<Multipart>(mimeMessage.Body);
            Assert.Equal("mixed", multipart.ContentType.MediaSubtype);
            Assert.Equal(2, multipart.Count);

            var bodyPart = Assert.IsType<TextPart>(multipart[0]);
            Assert.Equal("Message with attachment", bodyPart.Text);

            var attachmentPart = Assert.IsType<MimePart>(multipart[1]);
            Assert.Equal("test.pdf", attachmentPart.FileName);
        }
        finally
        {
            if (disposables != null)
            {
                foreach (var d in disposables)
                {
                    try { d?.Dispose(); } catch { /* ignore disposal errors in tests */ }
                }
            }
        }
    }

    // -------------------------- Default constructor wiring --------------------------

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

    // -------------------------- SMTP error handling --------------------------

    [Fact]
    public async Task SendAsync_WhenSmtpCommandException_ThrowsInvalidOperationException()
    {
        var adapter = new ThrowingSmtpClientAdapter(onSend: CreateSmtpCommandException());
        var factory = new ThrowingSmtpClientFactory(adapter);
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings(sendMail: true, withUser: false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.SendAsync(messageDto, settings));
    }

    [Fact]
    public async Task SendAsync_WhenProtocolException_ThrowsInvalidOperationException()
    {
        var adapter = new ThrowingSmtpClientAdapter(
            onSend: new TestProtocolException("protocol fail"));
        var factory = new ThrowingSmtpClientFactory(adapter);
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings(sendMail: true, withUser: false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.SendAsync(messageDto, settings));
    }

    [Fact]
    public async Task SendAsync_WhenIOException_Rethrows()
    {
        var adapter = new ThrowingSmtpClientAdapter(
            onSend: new IOException("io fail"));
        var factory = new ThrowingSmtpClientFactory(adapter);
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings(sendMail: true, withUser: false);

        await Assert.ThrowsAsync<IOException>(() => sut.SendAsync(messageDto, settings));
    }

    [Fact]
    public async Task SendAsync_WhenOperationCanceledException_Rethrows()
    {
        var adapter = new ThrowingSmtpClientAdapter(
            onSend: new OperationCanceledException("cancel"));
        var factory = new ThrowingSmtpClientFactory(adapter);
        var sut = new SendMail(factory);
        var messageDto = CreateBaseMessage();
        var settings = CreateBaseSettings(sendMail: true, withUser: false);

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.SendAsync(messageDto, settings));
    }
}
