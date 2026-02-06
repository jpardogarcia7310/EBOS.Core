using EBOS.Core.Mail;

namespace EBOS.Core.Test.Mail;

public sealed class FakeSmtpClientFactory : ISmtpClientFactory
{
    public FakeSmtpClientAdapter Instance { get; } = new();

    public ISmtpClientAdapter Create() => Instance;
    public ISmtpClientAdapter Create(SmtpClientOptions? options)
    {
        // Options are ignored in this fake, but they could be used
        // to simulate different behaviors in tests.
        return new FakeSmtpClientAdapter();
    }
}
