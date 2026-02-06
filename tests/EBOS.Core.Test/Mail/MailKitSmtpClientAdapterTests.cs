using EBOS.Core.Mail;
using MailKit.Net.Smtp;

namespace EBOS.Core.Test.Mail;

public class MailKitSmtpClientAdapterTests
{
    [Fact]
    public void Ctor_NullInner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MailKitSmtpClientAdapter(null!));
    }

    [Fact]
    public void Dispose_DoesNotThrowWhenNotConnected()
    {
        var adapter = new MailKitSmtpClientAdapter(new SmtpClient());

        adapter.Dispose();
    }
}
