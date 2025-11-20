namespace EBOS.Core.Mail;

public interface ISmtpClientFactory
{
    ISmtpClientAdapter Create();
}