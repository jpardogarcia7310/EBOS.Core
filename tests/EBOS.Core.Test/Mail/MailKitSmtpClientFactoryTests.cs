using EBOS.Core.Mail;
using MailKit.Net.Smtp;
using System.Net.Security;

namespace EBOS.Core.Test.Mail;

public class MailKitSmtpClientFactoryTests
{
    private static SmtpClient GetInnerClient(ISmtpClientAdapter adapter)
    {
        var field = adapter.GetType().GetField("_inner",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (field is null)
        {
            throw new InvalidOperationException("No se pudo acceder al SmtpClient interno.");
        }

        return (SmtpClient)field.GetValue(adapter)!;
    }

    [Fact]
    public void Create_ReturnsNonNullAdapter()
    {
        // Arrange
        var factory = new MailKitSmtpClientFactory();

        // Act
        var adapter = factory.Create();

        // Assert
        Assert.NotNull(adapter);
        Assert.IsType<ISmtpClientAdapter>(adapter, exactMatch: false);

        // Should not throw on dispose (no connection was made).
        adapter.Dispose();
    }

    [Fact]
    public void Create_ReturnsNewInstanceEachTime()
    {
        // Arrange
        var factory = new MailKitSmtpClientFactory();

        // Act
        var adapter1 = factory.Create();
        var adapter2 = factory.Create();

        // Assert
        Assert.NotSame(adapter1, adapter2);

        adapter1.Dispose();
        adapter2.Dispose();
    }

    [Fact]
    public void Create_WithTimeoutOption_SetsClientTimeout()
    {
        var factory = new MailKitSmtpClientFactory();
        var options = new SmtpClientOptions
        {
            TimeoutMilliseconds = 12345
        };

        var adapter = factory.Create(options);
        var inner = GetInnerClient(adapter);

        Assert.Equal(12345, inner.Timeout);

        adapter.Dispose();
    }

    [Fact]
    public void Create_WithCallbackOption_SetsServerCertificateValidationCallback()
    {
        var factory = new MailKitSmtpClientFactory();
        RemoteCertificateValidationCallback callback = (_, _, _, _) => false;

        var options = new SmtpClientOptions
        {
            ServerCertificateValidationCallback = callback
        };

        var adapter = factory.Create(options);
        var inner = GetInnerClient(adapter);

        Assert.Same(callback, inner.ServerCertificateValidationCallback);

        adapter.Dispose();
    }

    [Fact]
    public void Create_WithIgnoreCertificateValidation_OverridesCallback()
    {
        var factory = new MailKitSmtpClientFactory();
        RemoteCertificateValidationCallback callback = (_, _, _, _) => false;

        var options = new SmtpClientOptions
        {
            IgnoreCertificateValidation = true,
            ServerCertificateValidationCallback = callback
        };

        var adapter = factory.Create(options);
        var inner = GetInnerClient(adapter);

        Assert.NotNull(inner.ServerCertificateValidationCallback);
        Assert.True(inner.ServerCertificateValidationCallback!(default!, default!, default!, SslPolicyErrors.None));

        adapter.Dispose();
    }
}
