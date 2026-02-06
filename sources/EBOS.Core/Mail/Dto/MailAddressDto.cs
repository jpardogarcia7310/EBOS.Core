namespace EBOS.Core.Mail.Dto;

public class MailAddressDto(string name, string address)
{
    public string Name { get; set; } = name;
    public string Address { get; set; } = address;
}
