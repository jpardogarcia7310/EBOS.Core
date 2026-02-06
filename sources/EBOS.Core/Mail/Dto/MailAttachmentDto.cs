using System.Diagnostics.CodeAnalysis;

namespace EBOS.Core.Mail.Dto;

public class MailAttachmentDto
{
    public string MediaType { get; set; } = string.Empty;
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays",
        Justification = "Transport DTO for mail attachments; byte[] is required.")]
    public byte[] Content { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
}
