using EBOS.Core.Mail.Dto;

namespace EBOS.Core.Mail;

/// <summary>
/// Interfaz mínima para el servicio de envío de correo.
/// </summary>
public interface ISendMail
{
    Task SendAsync(MailMessageDto mailMessageDto, MailSettingsDto mailSettings);
}