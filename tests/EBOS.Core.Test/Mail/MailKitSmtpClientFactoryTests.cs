using EBOS.Core.Mail;

namespace EBOS.Core.Test.Mail;

public class MailKitSmtpClientFactoryTests
{
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

        // No debe lanzar al desecharlo (no se ha conectado a nada)
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
}