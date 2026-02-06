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

    /// <summary>
    /// Asynchronous dispose for the adapter.
    /// Tries to disconnect asynchronously and then releases the client synchronously.
    /// Catches expected exceptions during shutdown and rethrows any other exception.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        try
        {
            await _inner.DisconnectAsync(true).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // If the operation was canceled, propagate the cancellation so the caller knows.
            throw;
        }
        catch (IOException)
        {
            // I/O errors during Disconnect are relatively common on unstable networks;
            // ignore them here to avoid masking the original flow exception,
            // but do not rethrow to allow resource cleanup.
        }
        catch (MailKit.Net.Smtp.SmtpCommandException)
        {
            // SMTP command exceptions during shutdown may occur; we do not propagate them.
        }
        catch (MailKit.ProtocolException)
        {
            // MailKit protocol errors during Disconnect; we do not propagate them.
        }
        catch (Exception)
        {
            // For any other unexpected exception, rethrow to avoid hiding serious failures.
            throw;
        }

        // Final synchronous release (compatible with all MailKit versions).
        _inner.Dispose();
    }
}
