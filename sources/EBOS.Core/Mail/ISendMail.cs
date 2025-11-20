using EBOS.Core.Mail.Dto;

namespace EBOS.Core.Mail;

public interface ISendMail
{
    Task SendAsync(MailMessageDto mailMessageDto, MailSettingsDto mailSettings);
}
