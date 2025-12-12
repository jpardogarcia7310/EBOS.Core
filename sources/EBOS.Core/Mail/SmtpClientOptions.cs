using System.Net.Security;

namespace EBOS.Core.Mail;

/// <summary>
/// Opciones para configurar el SmtpClient creado por la fábrica.
/// </summary>
public sealed class SmtpClientOptions
{
    /// <summary>
    /// Timeout en milisegundos. Si es null, se usa el valor por defecto del cliente.
    /// </summary>
    public int? TimeoutMilliseconds { get; set; }

    /// <summary>
    /// Callback para validar el certificado del servidor. Si es null, se usa la validación por defecto.
    /// </summary>
    public RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }

    /// <summary>
    /// Permite indicar si se debe ignorar la validación del certificado (no recomendado en producción).
    /// Si se establece a true, se asigna un callback que siempre devuelve true.
    /// </summary>
    public bool IgnoreCertificateValidation { get; set; }

    /// <summary>
    /// Crea una instancia con valores por defecto.
    /// </summary>
    public static SmtpClientOptions Default() => new();
}