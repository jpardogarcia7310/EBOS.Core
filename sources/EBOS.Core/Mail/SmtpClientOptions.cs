using System.Net.Security;

namespace EBOS.Core.Mail;

/// <summary>
/// Options to configure the SmtpClient created by the factory.
/// </summary>
public sealed class SmtpClientOptions
{
    /// <summary>
    /// Timeout in milliseconds. When null, the client default is used.
    /// </summary>
    public int? TimeoutMilliseconds { get; set; }

    /// <summary>
    /// Callback to validate the server certificate. When null, the default validation is used.
    /// </summary>
    public RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }

    /// <summary>
    /// Indicates whether certificate validation should be ignored (not recommended in production).
    /// When true, a callback that always returns true is assigned.
    /// </summary>
    public bool IgnoreCertificateValidation { get; set; }

    /// <summary>
    /// Creates an instance with default values.
    /// </summary>
    public static SmtpClientOptions Default() => new();
}
