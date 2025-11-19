using MailKit.Net.Smtp;

namespace EBOS.Core.Mail;

public sealed class MailKitSmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClientAdapter Create() => new MailKitSmtpClientAdapter(new SmtpClient());
}