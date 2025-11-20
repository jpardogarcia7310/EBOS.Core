using EBOS.Core.Mail;
using MimeKit;

namespace EBOS.Core.Test.Mail;

public sealed class FakeSmtpClientAdapter : ISmtpClientAdapter
{
    public bool ConnectCalled { get; private set; }
    public bool AuthenticateCalled { get; private set; }
    public bool SendCalled { get; private set; }
    public bool DisconnectCalled { get; private set; }
    public bool Disposed { get; private set; }
    public string? Host { get; private set; }
    public int Port { get; private set; }
    public bool UseSsl { get; private set; }
    public string? UserName { get; private set; }
    public string? Password { get; private set; }
    public MimeMessage? LastMessage { get; private set; }

    public Task ConnectAsync(string host, int port, bool useSsl, CancellationToken cancellationToken = default)
    {
        ConnectCalled = true;
        Host = host;
        Port = port;
        UseSsl = useSsl;

        return Task.CompletedTask;
    }

    public Task AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        AuthenticateCalled = true;
        UserName = userName;
        Password = password;

        return Task.CompletedTask;
    }

    public Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default)
    {
        SendCalled = true;

        // Guardamos una copia independiente del mensaje para inspeccionarla en los tests
        using var ms = new MemoryStream();
        message.WriteTo(ms, cancellationToken);
        ms.Position = 0;
        LastMessage = MimeMessage.Load(ms, cancellationToken);

        return Task.CompletedTask;
    }

    public Task DisconnectAsync(bool quit, CancellationToken cancellationToken = default)
    {
        DisconnectCalled = true;

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Disposed = true;
    }
}