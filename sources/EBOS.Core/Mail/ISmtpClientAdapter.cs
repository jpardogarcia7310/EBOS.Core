using MimeKit;

namespace EBOS.Core.Mail;

public interface ISmtpClientAdapter : IDisposable, IAsyncDisposable
{
    Task ConnectAsync(string host, int port, bool useSsl, CancellationToken cancellationToken = default);
    Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default);
    Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default);
    Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default);
}