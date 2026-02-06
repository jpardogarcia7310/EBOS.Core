using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Primitives;

public class ErrorResult(string message, int code, string? exceptionMsg = null) : IErrorResult
{
    public int Code { get; set; } = code;
    public string? ExceptionMsg { get; set; } = exceptionMsg;
    public string Message { get; set; } = message ?? throw new ArgumentNullException(nameof(message));
}