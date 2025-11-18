using EBOS.Core.Primitives;

namespace EBOS.Core.Extensions;

public static class OperationResultExtensions
{
    private const string OperationResultCannotBeNull = "operationResult cannot be null";

    public static OperationResult<TResult> AddResult<TResult>(this OperationResult<TResult> operationResult, TResult result)
    {
        if (operationResult is null)
            throw new ArgumentNullException(nameof(operationResult), OperationResultCannotBeNull);

        operationResult.Result = result;

        return operationResult;
    }

    public static OperationResult<TResult> AddResultWithError<TResult>(this OperationResult<TResult> operationResult,
        TResult result, string errorMessage, int errorCode, string exceptionMsg)
    {
        if (operationResult is null)
            throw new ArgumentNullException(nameof(operationResult), OperationResultCannotBeNull);

        operationResult.Result = result;
        operationResult.Errors.Add(new ErrorResult(errorMessage, errorCode, exceptionMsg));

        return operationResult;
    }

    public static OperationResult<TResult> AddResultWithError<TResult>(this OperationResult<TResult> operationResult,
        TResult result, ErrorResult errorResult)
    {
        if (operationResult is null)
            throw new ArgumentNullException(nameof(operationResult), OperationResultCannotBeNull);

        operationResult.Result = result;
        operationResult.Errors.Add(errorResult);

        return operationResult;
    }

    public static OperationResult<TResult> AddError<TResult>(this OperationResult<TResult> operationResult,
        string errorMessage, int errorCode, string? exceptionMsg)
    {
        if (operationResult is null)
            throw new ArgumentNullException(nameof(operationResult), OperationResultCannotBeNull);

        operationResult.Errors.Add(new ErrorResult(errorMessage, errorCode, exceptionMsg));

        return operationResult;
    }

    public static OperationResult<TResult> AddError<TResult>(this OperationResult<TResult> operationResult, ErrorResult errorResult)
    {
        if (operationResult is null)
            throw new ArgumentNullException(nameof(operationResult), OperationResultCannotBeNull);

        operationResult.Errors.Add(errorResult);

        return operationResult;
    }

    public static OperationResult<TResult> AddErrors<TResult>(this OperationResult<TResult> operationResult, IEnumerable<ErrorResult> validationErrors)
    {
        if (operationResult is null)
            throw new ArgumentNullException(nameof(operationResult), OperationResultCannotBeNull);
        if (validationErrors is null)
            return operationResult;

        foreach (var error in validationErrors)
            operationResult.Errors.Add(error);

        return operationResult;
    }
}