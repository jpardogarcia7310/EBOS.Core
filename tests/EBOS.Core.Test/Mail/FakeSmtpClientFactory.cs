using EBOS.Core.Mail;

namespace EBOS.Core.Test.Mail;

public sealed class FakeSmtpClientFactory : ISmtpClientFactory
{
    public FakeSmtpClientAdapter Instance { get; } = new();

    public ISmtpClientAdapter Create() => Instance;
    public ISmtpClientAdapter Create(SmtpClientOptions? options)
    {
        // Las opciones se ignoran en este fake, pero se podrían usar
        // para simular distintos comportamientos en tests.
        return new FakeSmtpClientAdapter();
    }
}