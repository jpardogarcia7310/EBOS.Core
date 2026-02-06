using MailKit.Net.Smtp;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;

namespace EBOS.Core.Mail;

public sealed class MailKitSmtpClientFactory : ISmtpClientFactory
{
    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "The SmtpClient is transferred to the adapter and must be disposed by the consumer.")]
    public ISmtpClientAdapter Create()
        => Create(null);

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "The SmtpClient is transferred to the adapter and must be disposed by the consumer.")]
    public ISmtpClientAdapter Create(SmtpClientOptions? options)
    {
        var client = new SmtpClient();

        if (options?.TimeoutMilliseconds is int timeout)
        {
            client.Timeout = timeout;
        }

        if (options?.IgnoreCertificateValidation == true)
        {
#pragma warning disable CA5359 // Caller explicitly requests disabling certificate validation.
            client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
#pragma warning restore CA5359
        }
        else if (options?.ServerCertificateValidationCallback is RemoteCertificateValidationCallback callback)
        {
            client.ServerCertificateValidationCallback = callback;
        }

        return new MailKitSmtpClientAdapter(client);
    }
}
