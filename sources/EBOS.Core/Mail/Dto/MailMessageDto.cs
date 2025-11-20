using System.Collections.ObjectModel;

namespace EBOS.Core.Mail.Dto;

public class MailMessageDto
{
    // Colecciones solo lectura (la referencia), pero mutables por dentro.
    public Collection<MailAddressDto> FromAddress { get; } = [];
    public Collection<MailAddressDto> ToAddress { get; } = [];

    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;

    public MailAttachmentDto? MailAttachment { get; set; }
}
