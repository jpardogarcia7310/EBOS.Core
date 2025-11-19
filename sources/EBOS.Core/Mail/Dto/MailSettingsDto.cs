namespace EBOS.Core.Mail.Dto;

public class MailSettingsDto
{
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string MailUser { get; set; } = string.Empty;
    public string MailPassword { get; set; } = string.Empty;
    public bool HasSSL { get; set; }
    public bool SendMail { get; set; }
}
