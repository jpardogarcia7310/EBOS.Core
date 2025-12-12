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
    /// Dispose asíncrono del adaptador.
    /// Intenta desconectar de forma asíncrona y luego libera el cliente de forma síncrona.
    /// Captura excepciones específicas esperables durante el cierre y relanza cualquier otra excepción.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        try
        {
            await _inner.DisconnectAsync(true).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Si la operación fue cancelada, propagar la cancelación para que el llamador lo sepa.
            throw;
        }
        catch (IOException)
        {
            // Errores de E/S durante el Disconnect son relativamente comunes en redes inestables;
            // los ignoramos aquí para no ocultar la excepción original del flujo de trabajo,
            // pero no re-lanzamos para permitir la liberación de recursos.
        }
        catch (MailKit.Net.Smtp.SmtpCommandException)
        {
            // Excepciones de comando SMTP durante el cierre pueden ocurrir; no las propagamos.
        }
        catch (MailKit.ProtocolException)
        {
            // Errores de protocolo de MailKit durante el Disconnect; no los propagamos.
        }
        catch (Exception)
        {
            // Para cualquier otra excepción inesperada, re-lanzamos para no ocultar fallos graves.
            throw;
        }

        // Liberación final sincrónica (compatible con todas las versiones de MailKit)
        _inner.Dispose();
    }
}