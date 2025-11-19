using MailKit.Net.Smtp;
using MimeKit;

namespace EBOS.Core.Mail;

public sealed class MailKitSmtpClientAdapter(SmtpClient inner) : ISmtpClientAdapter
{
    public Task ConnectAsync(string host, int port, bool useSsl, CancellationToken cancellationToken = default) =>
        inner.ConnectAsync(host, port, useSsl, cancellationToken);

    public Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default) =>
        inner.AuthenticateAsync(userName, password, cancellationToken);

    public Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default) =>
        inner.SendAsync(message, cancellationToken);

    public Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default) =>
        inner.DisconnectAsync(quit, cancellationToken);

    public void Dispose() => inner.Dispose();
}