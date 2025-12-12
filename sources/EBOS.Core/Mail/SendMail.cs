using EBOS.Core.Mail.Dto;
using MailKit;
using MimeKit;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace EBOS.Core.Mail;

public class SendMail(ISmtpClientFactory smtpClientFactory, ILogger<SendMail>? logger = null) : ISendMail
{
    private readonly ISmtpClientFactory _smtpClientFactory = smtpClientFactory ?? throw new ArgumentNullException(nameof(smtpClientFactory));

    public SendMail() : this(new MailKitSmtpClientFactory(), logger: null)
    { }

    public async Task SendAsync(MailMessageDto mailMessageDto, MailSettingsDto mailSettings)
    {
        if (mailMessageDto is null)
            throw new ArgumentNullException(nameof(mailMessageDto));
        if (mailSettings is null)
            throw new ArgumentNullException(nameof(mailSettings));
        if (!mailSettings.SendMail)
            return;

        ValidateAddresses(mailMessageDto);
        ValidateSubject(mailMessageDto);
        EnsureBodyType(mailMessageDto);
        ValidateAttachment(mailMessageDto.MailAttachment);

        var (message, disposables) = CreateMessage(mailMessageDto);

        // Crear el cliente y asegurarnos de que se DisposeAsync al salir
        await using var client = _smtpClientFactory.Create();

        try
        {
            await SendMessageAsync(mailSettings, client, message).ConfigureAwait(false);
        }
        finally
        {
            if (disposables is not null)
            {
                foreach (var d in disposables)
                {
                    try
                    {
                        d?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.DisposeResourceWarning(logger, ex);
                    }
                }
            }
        }
    }

    private static void ValidateAddresses(MailMessageDto mailMessageDto)
    {
        if (mailMessageDto.FromAddress is null || mailMessageDto.FromAddress.Count == 0)
            throw new InvalidOperationException("MailMessageDto must have at least one From address.");
        if (mailMessageDto.ToAddress is null || mailMessageDto.ToAddress.Count == 0)
            throw new InvalidOperationException("MailMessageDto must have at least one To address.");
    }

    private static void ValidateSubject(MailMessageDto mailMessageDto)
    {
        if (string.IsNullOrWhiteSpace(mailMessageDto.Subject))
            throw new InvalidOperationException("MailMessageDto.Subject cannot be null or empty.");
    }

    private static void EnsureBodyType(MailMessageDto mailMessageDto)
    {
        if (string.IsNullOrWhiteSpace(mailMessageDto.BodyType))
            mailMessageDto.BodyType = "plain";
    }

    private static void ValidateAttachment(MailAttachmentDto? attachment)
    {
        if (attachment is null)
            return;
        if (attachment.Content is null || attachment.Content.Length == 0)
            throw new InvalidOperationException("Attachment content cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(attachment.MediaType))
            throw new InvalidOperationException("Attachment media type is required.");
        if (string.IsNullOrWhiteSpace(attachment.FileName))
            throw new InvalidOperationException("Attachment file name is required.");
    }

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "Los disposables creados se devuelven al llamador para su liberación controlada.")]
    private static (MimeMessage Message, List<IDisposable> Disposables) CreateMessage(MailMessageDto mailMessageDto)
    {
        var disposables = new List<IDisposable>();

        var message = new MimeMessage();

        message.From.AddRange(
            mailMessageDto.FromAddress.Select(ma => new MailboxAddress(ma.Name, ma.Address)));
        message.To.AddRange(
            mailMessageDto.ToAddress.Select(ma => new MailboxAddress(ma.Name, ma.Address)));
        message.Subject = mailMessageDto.Subject;

        var body = new TextPart(mailMessageDto.BodyType)
        {
            Text = mailMessageDto.Message ?? string.Empty
        };

        if (mailMessageDto.MailAttachment is null)
        {
            message.Body = body;
            return (message, disposables);
        }

        var attachmentDto = mailMessageDto.MailAttachment;

        // Crear MemoryStream de solo lectura y añadirlo a disposables para su liberación posterior
        var ms = new MemoryStream(attachmentDto.Content, writable: false);
        disposables.Add(ms);

        var attachment = new MimePart(attachmentDto.MediaType)
        {
            FileName = attachmentDto.FileName,
            Content = new MimeContent(ms, ContentEncoding.Default),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64
        };

        var multipart = new Multipart("mixed")
        {
            body,
            attachment
        };
        message.Body = multipart;

        return (message, disposables);
    }

    /// <summary>
    /// Método de instancia que realiza la conexión, autenticación, envío y desconexión.
    /// Registra IOException antes de relanzarla para diagnóstico.
    /// </summary>
    private async Task SendMessageAsync(MailSettingsDto mailSettings,
        ISmtpClientAdapter client, MimeMessage message)
    {
        try
        {
            await client
                .ConnectAsync(mailSettings.Server, mailSettings.Port, mailSettings.HasSSL)
                .ConfigureAwait(false);

            if (!string.IsNullOrEmpty(mailSettings.MailUser))
            {
                await client
                    .AuthenticateAsync(mailSettings.MailUser, mailSettings.MailPassword)
                    .ConfigureAwait(false);
            }

            await client.SendAsync(message).ConfigureAwait(false);
            await client.DisconnectAsync(true).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (IOException ioEx)
        {
            Log.IoErrorSendingEmail(logger, mailSettings.Server, mailSettings.Port, ioEx);
            throw;
        }
        catch (MailKit.Net.Smtp.SmtpCommandException smtpCmdEx)
        {
            throw new InvalidOperationException("SMTP command failed during send.", smtpCmdEx);
        }
        catch (ProtocolException protocolEx)
        {
            throw new InvalidOperationException("SMTP protocol error during send.", protocolEx);
        }
    }

    /// <summary>
    /// Clase interna con delegados LoggerMessage para evitar formateo innecesario.
    /// </summary>
    private static class Log
    {
        private static readonly Action<ILogger, Exception?> _disposeResourceWarning =
            LoggerMessage.Define(
                LogLevel.Warning,
                new EventId(1001, "DisposeResourceWarning"),
                "Error disposing resource created for mail message; continuing disposal of remaining resources.");

        private static readonly Action<ILogger, string, int, Exception?> _ioErrorSendingEmail =
            LoggerMessage.Define<string, int>(
                LogLevel.Error,
                new EventId(1002, "IoErrorSendingEmail"),
                "I/O error occurred while sending email to server {Server}:{Port}");

        public static void DisposeResourceWarning(ILogger? logger, Exception? ex)
        {
            if (logger is null) return;
            _disposeResourceWarning(logger, ex);
        }

        public static void IoErrorSendingEmail(ILogger? logger, string server, int port, Exception? ex)
        {
            if (logger is null) return;
            _ioErrorSendingEmail(logger, server, port, ex);
        }
    }
}