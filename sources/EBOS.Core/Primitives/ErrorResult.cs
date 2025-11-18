using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Primitives;

public class ErrorResult(string message, int code, string? exceptionMsg) : IErrorResult
{
    public string? ExceptionMsg { get; set; } = exceptionMsg;
    public int Code { get; set; } = code;
    public string Message { get; set; } = message;
}
