using System.Diagnostics.CodeAnalysis;

namespace EBOS.Core.Mail.Dto;

public class MailAttachmentDto
{
    public string MediaType { get; set; } = string.Empty;
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays",
        Justification = "DTO de transporte para adjuntos de correo; se necesita byte[].")]
    public byte[] Content { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
}
