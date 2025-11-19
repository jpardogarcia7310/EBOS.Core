using EBOS.Core.Mail;

namespace EBOS.Core.Test.Mail;

public sealed class FakeSmtpClientFactory : ISmtpClientFactory
{
    public FakeSmtpClientAdapter Instance { get; } = new();

    public ISmtpClientAdapter Create() => Instance;
}