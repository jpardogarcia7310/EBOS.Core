using MailKit.Net.Smtp;
using System.Diagnostics.CodeAnalysis;

namespace EBOS.Core.Mail;

public sealed class MailKitSmtpClientFactory : ISmtpClientFactory
{
    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "La propiedad del SmtpClient se transfiere a MailKitSmtpClientAdapter, " +
                        "que implementa IDisposable y es desechado por el consumidor.")]
    public ISmtpClientAdapter Create() => new MailKitSmtpClientAdapter(new SmtpClient());
}