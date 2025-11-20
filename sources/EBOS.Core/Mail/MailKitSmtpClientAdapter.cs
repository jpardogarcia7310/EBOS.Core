using MailKit.Net.Smtp;
using MimeKit;

namespace EBOS.Core.Mail;

public sealed class MailKitSmtpClientAdapter(SmtpClient inner) : ISmtpClientAdapter
{
    private readonly SmtpClient _inner = inner ?? throw new ArgumentNullException(nameof(inner));

    public Task ConnectAsync(string host, int port, bool useSsl, CancellationToken cancellationToken = default) =>
        _inner.ConnectAsync(host, port, useSsl, cancellationToken);

    public Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default) =>
        _inner.AuthenticateAsync(userName, password, cancellationToken);

    public Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default) =>
        _inner.SendAsync(message, cancellationToken);

    public Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default) =>
        _inner.DisconnectAsync(quit, cancellationToken);

    public void Dispose() => _inner.Dispose();
}