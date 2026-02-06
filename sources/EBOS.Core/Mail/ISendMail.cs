using EBOS.Core.Mail.Dto;

namespace EBOS.Core.Mail;

/// <summary>
/// Minimal interface for the mail sending service.
/// </summary>
public interface ISendMail
{
    Task SendAsync(MailMessageDto mailMessageDto, MailSettingsDto mailSettings);
}
