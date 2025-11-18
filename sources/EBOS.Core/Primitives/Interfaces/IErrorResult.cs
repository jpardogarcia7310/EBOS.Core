namespace EBOS.Core.Primitives.Interfaces;

public interface IErrorResult
{
    int Code { get; set; }
    string? ExceptionMsg { get; set; }
    string Message { get; set; }
}