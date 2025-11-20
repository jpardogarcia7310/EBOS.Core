using EBOS.Core.Mail.Dto;
using MimeKit;
using System.Diagnostics.CodeAnalysis;

namespace EBOS.Core.Mail;

public class SendMail(ISmtpClientFactory smtpClientFactory) : ISendMail
{
    private readonly ISmtpClientFactory _smtpClientFactory = smtpClientFactory ?? throw new ArgumentNullException(nameof(smtpClientFactory));

    public SendMail() : this(new MailKitSmtpClientFactory())
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
        using var message = CreateMessage(mailMessageDto);
        using var client = _smtpClientFactory.Create();

        await SendMessageAsync(mailSettings, client, message).ConfigureAwait(false);
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
            // Valor por defecto: texto plano
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
        Justification = "TextPart, MimePart y Multipart pasan a ser propiedad de MimeMessage, " +
                        "que se desecha mediante using en SendAsync.")]
    private static MimeMessage CreateMessage(MailMessageDto mailMessageDto)
    {
        var message = new MimeMessage();

        message.From.AddRange(
            mailMessageDto.FromAddress.Select(ma => new MailboxAddress(ma.Name, ma.Address)));
        message.To.AddRange(
            mailMessageDto.ToAddress.Select(ma => new MailboxAddress(ma.Name, ma.Address)));
        message.Subject = mailMessageDto.Subject;
        var body = new TextPart(mailMessageDto.BodyType)
        {
            Text = mailMessageDto.Message
        };
        if (mailMessageDto.MailAttachment is null)
        {
            message.Body = body;
            return message;
        }
        var attachmentDto = mailMessageDto.MailAttachment;
        var attachment = new MimePart(attachmentDto.MediaType)
        {
            FileName = attachmentDto.FileName,
            Content = new MimeContent(new MemoryStream(attachmentDto.Content)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64
        };
        var multipart = new Multipart("mixed")
        {
            body,
            attachment
        };
        message.Body = multipart;

        return message;
    }

    private static async Task SendMessageAsync(MailSettingsDto mailSettings,
        ISmtpClientAdapter client, MimeMessage message)
    {
        await client
            .ConnectAsync(mailSettings.Server, mailSettings.Port, mailSettings.HasSSL)
            .ConfigureAwait(false);
        if (!string.IsNullOrEmpty(mailSettings.MailUser))
            await client
                .AuthenticateAsync(mailSettings.MailUser, mailSettings.MailPassword)
                .ConfigureAwait(false);
        await client.SendAsync(message).ConfigureAwait(false);
        await client.DisconnectAsync(true).ConfigureAwait(false);
    }
}