using EBOS.Core.Mail;

namespace EBOS.Core.Test.Mail;

public class SmtpClientOptionsTests
{
    [Fact]
    public void Default_ReturnsNewInstanceWithDefaults()
    {
        var options = SmtpClientOptions.Default();

        Assert.NotNull(options);
        Assert.Null(options.TimeoutMilliseconds);
        Assert.Null(options.ServerCertificateValidationCallback);
        Assert.False(options.IgnoreCertificateValidation);
    }
}
