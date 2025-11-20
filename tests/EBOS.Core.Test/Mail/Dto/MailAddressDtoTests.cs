using EBOS.Core.Mail.Dto;

namespace EBOS.Core.Test.Mail.Dto;

public class MailAddressDtoTests
{
    [Fact]
    public void Ctor_SetsNameAndAddress()
    {
        // Arrange
        const string name = "Remitente";
        const string address = "remitente@ejemplo.com";

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
        var dto = new MailAddressDto("OldName", "old@ejemplo.com")
        {
            // Act
            Name = "NewName",
            Address = "new@ejemplo.com"
        };

        // Assert
        Assert.Equal("NewName", dto.Name);
        Assert.Equal("new@ejemplo.com", dto.Address);
    }
}