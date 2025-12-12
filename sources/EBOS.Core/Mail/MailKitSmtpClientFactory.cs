using MailKit.Net.Smtp;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;

namespace EBOS.Core.Mail;

public sealed class MailKitSmtpClientFactory : ISmtpClientFactory
{
    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "El SmtpClient se transfiere al adaptador y debe ser desechado por el consumidor.")]
    public ISmtpClientAdapter Create()
        => Create(null);

    [SuppressMessage(
        "Reliability",
        "CA2000:Dispose objects before losing scope",
        Justification = "El SmtpClient se transfiere al adaptador y debe ser desechado por el consumidor.")]
    public ISmtpClientAdapter Create(SmtpClientOptions? options)
    {
        var client = new SmtpClient();

        if (options?.TimeoutMilliseconds is int timeout)
        {
            client.Timeout = timeout;
        }

        if (options?.IgnoreCertificateValidation == true)
        {
            client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }
        else if (options?.ServerCertificateValidationCallback is RemoteCertificateValidationCallback callback)
        {
            client.ServerCertificateValidationCallback = callback;
        }

        return new MailKitSmtpClientAdapter(client);
    }
}