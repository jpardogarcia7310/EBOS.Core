using EBOS.Core.Mail.Dto;

namespace EBOS.Core.Test.Mail.Dto;

public class MailAddressDtoTests
{
    [Fact]
    public void Ctor_SetsNameAndAddress()
    {
        // Arrange
        const string name = "Sender";
        const string address = "sender@example.com";

        // Act
        var dto = new MailAddressDto(name, address);

        // Assert
        Assert.Equal(name, dto.Name);
        Assert.Equal(address, dto.Address);
    }

    [Fact]
    public void Properties_AreMutable()
    {
        // Arrange
        var dto = new MailAddressDto("OldName", "old@example.com")
        {
            // Act
            Name = "NewName",
            Address = "new@example.com"
        };

        // Assert
        Assert.Equal("NewName", dto.Name);
        Assert.Equal("new@example.com", dto.Address);
    }
}
