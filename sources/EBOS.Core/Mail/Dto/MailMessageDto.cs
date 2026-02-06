using System.Collections.ObjectModel;

namespace EBOS.Core.Mail.Dto;

/// <summary>
/// Minimal DTO to represent an email message.
/// Adjust or extend based on your actual needs.
/// </summary>
public class MailMessageDto
{
    // Read-only collections (by reference), but mutable internally.
    public Collection<MailAddressDto> FromAddress { get; } = [];
    public Collection<MailAddressDto> ToAddress { get; } = [];

    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;

    public MailAttachmentDto? MailAttachment { get; set; }
}
