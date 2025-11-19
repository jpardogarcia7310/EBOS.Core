using EBOS.Core.Extensions;
using EBOS.Core.Primitives;

namespace EBOS.Core.Test.Extensions;

public class OperationResultExtensionsTests
{
    #region Helpers
    private static OperationResult<string> CreateEmptyStringResult() => new();

    private static OperationResult<int> CreateEmptyIntResult() => new();
    #endregion

    #region AddResult
    [Fact]
    public void AddResult_NullOperationResult_ThrowsArgumentNullException()
    {
        OperationResult<string>? operationResult = null;
        var ex = Assert.Throws<ArgumentNullException>(() =>
            OperationResultExtensions.AddResult(operationResult!, "value"));

        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddResult_SetsResultAndReturnsSameInstance()
    {
        var operationResult = CreateEmptyStringResult();
        var returned = operationResult.AddResult("test");

        Assert.Same(operationResult, returned);
        Assert.Equal("test", operationResult.Result);
    }

    [Fact]
    public void AddResult_DoesNotModifyExistingErrors()
    {
        var operationResult = CreateEmptyStringResult();

        operationResult.Errors.Add(new ErrorResult("e1", 1, "ex"));
        operationResult.AddResult("updated");

        Assert.Equal("updated", operationResult.Result);
        Assert.Single(operationResult.Errors);
    }
    #endregion

    #region AddResultWithError (message, code, exceptionMsg)
    [Fact]
    public void AddResultWithError_Params_NullOperationResult_ThrowsArgumentNullException()
    {
        OperationResult<string>? operationResult = null;
        var ex = Assert.Throws<ArgumentNullException>(() =>
            OperationResultExtensions.AddResultWithError(operationResult!, "value", "error", 1, "ex"));

        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddResultWithError_Params_SetsResultAndAddsOneError()
    {
        var operationResult = CreateEmptyStringResult();
        var returned = operationResult.AddResultWithError("value", "error message", 100, "exception");

        Assert.Same(operationResult, returned);
        Assert.Equal("value", operationResult.Result);
        Assert.Single(operationResult.Errors);
        Assert.NotNull(operationResult.Errors.First()); // un ErrorResult cualquiera
    }

    [Fact]
    public void AddResultWithError_Params_AppendsErrorToExistingErrors()
    {
        var operationResult = CreateEmptyStringResult();

        operationResult.Errors.Add(new ErrorResult("existing", 1, "ex1"));
        operationResult.AddResultWithError("value", "new error", 2, "ex2");

        Assert.Equal("value", operationResult.Result);
        Assert.Equal(2, operationResult.Errors.Count);
    }
    #endregion

    #region AddResultWithError (ErrorResult)
    [Fact]
    public void AddResultWithError_ErrorResult_NullOperationResult_ThrowsArgumentNullException()
    {
        OperationResult<int>? operationResult = null;
        var errorResult = new ErrorResult("error", 1, "ex");
        var ex = Assert.Throws<ArgumentNullException>(() =>
            OperationResultExtensions.AddResultWithError(operationResult!, 10, errorResult));

        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddResultWithError_ErrorResult_SetsResultAndAddsGivenErrorInstance()
    {
        var operationResult = CreateEmptyIntResult();
        var error = new ErrorResult("error", 1, "ex");

        var returned = operationResult.AddResultWithError(42, error);

        Assert.Same(operationResult, returned);
        Assert.Equal(42, operationResult.Result);
        Assert.Single(operationResult.Errors);
        Assert.Same(error, operationResult.Errors.First());
    }

    [Fact]
    public void AddResultWithError_ErrorResult_AllowsNullErrorToBeAdded()
    {
        var operationResult = CreateEmptyIntResult();
        ErrorResult? error = null;

        operationResult.AddResultWithError(1, error!);

        Assert.Equal(1, operationResult.Result);
        Assert.Single(operationResult.Errors);
        Assert.Null(operationResult.Errors.First());
    }
    #endregion

    #region AddError (message, code, exceptionMsg)
    [Fact]
    public void AddError_Params_NullOperationResult_ThrowsArgumentNullException()
    {
        OperationResult<string>? operationResult = null;
        var ex = Assert.Throws<ArgumentNullException>(() =>
            OperationResultExtensions.AddError(operationResult!, "error", 5, "ex"));

        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddError_Params_AddsSingleError_AndDoesNotTouchResult()
    {
        var operationResult = CreateEmptyStringResult();
    
        operationResult.Result = "original";

        var returned = operationResult.AddError("error message", 10, "exception");

        Assert.Same(operationResult, returned);
        Assert.Equal("original", operationResult.Result);
        Assert.Single(operationResult.Errors);
        Assert.NotNull(operationResult.Errors.First());
    }

    [Fact]
    public void AddError_Params_AppendsToExistingErrors()
    {
        var operationResult = CreateEmptyStringResult();

        operationResult.Errors.Add(new ErrorResult("e1", 1, "ex1"));
        operationResult.AddError("e2", 2, "ex2");

        Assert.Equal(2, operationResult.Errors.Count);
    }

    [Fact]
    public void AddError_Params_AllowsNullExceptionMessage()
    {
        var operationResult = CreateEmptyStringResult();
        var returned = operationResult.AddError("error", 1, null);

        Assert.Same(operationResult, returned);
        Assert.Single(operationResult.Errors);
        // aquí solo verificamos que hay un error añadido; los detalles internos son de ErrorResult
    }
    #endregion

    #region AddError (ErrorResult)
    [Fact]
    public void AddError_ErrorResult_NullOperationResult_ThrowsArgumentNullException()
    {
        OperationResult<string>? operationResult = null;
        var error = new ErrorResult("error", 1, "ex");
        var ex = Assert.Throws<ArgumentNullException>(() =>
            OperationResultExtensions.AddError(operationResult!, error));

        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddError_ErrorResult_AddsGivenErrorInstance()
    {
        var operationResult = CreateEmptyStringResult();
        var error = new ErrorResult("error", 1, "ex");
        var returned = operationResult.AddError(error);

        Assert.Same(operationResult, returned);
        Assert.Single(operationResult.Errors);
        Assert.Same(error, operationResult.Errors.First());
    }

    [Fact]
    public void AddError_ErrorResult_AllowsNullErrorToBeAdded()
    {
        var operationResult = CreateEmptyStringResult();
        ErrorResult? error = null;

        operationResult.AddError(error!);

        Assert.Single(operationResult.Errors);
        Assert.Null(operationResult.Errors.First());
    }
    #endregion

    #region AddErrors
    [Fact]
    public void AddErrors_NullOperationResult_ThrowsArgumentNullException()
    {
        OperationResult<string>? operationResult = null;
        var errors = new List<ErrorResult>
        {
            new("e1", 1, "ex1")
        };
        var ex = Assert.Throws<ArgumentNullException>(() =>
            OperationResultExtensions.AddErrors(operationResult!, errors));

        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddErrors_NullValidationErrors_ReturnsSameInstanceWithoutChanges()
    {
        var operationResult = CreateEmptyStringResult();
 
        operationResult.Errors.Add(new ErrorResult("existing", 1, "ex"));

        IEnumerable<ErrorResult>? validationErrors = null;
        var returned = operationResult.AddErrors(validationErrors!);

        Assert.Same(operationResult, returned);
        Assert.Single(operationResult.Errors);
    }

    [Fact]
    public void AddErrors_EmptyValidationErrors_DoesNotChangeErrors()
    {
        var operationResult = CreateEmptyStringResult();

        operationResult.Errors.Add(new ErrorResult("existing", 1, "ex"));

        var validationErrors = new List<ErrorResult>();
        var returned = operationResult.AddErrors(validationErrors);

        Assert.Same(operationResult, returned);
        Assert.Single(operationResult.Errors);
    }

    [Fact]
    public void AddErrors_AddsAllErrorsToOperationResult()
    {
        var operationResult = CreateEmptyStringResult();

        var e1 = new ErrorResult("e1", 1, "ex1");
        var e2 = new ErrorResult("e2", 2, "ex2");

        var validationErrors = new List<ErrorResult> { e1, e2 };

        var returned = operationResult.AddErrors(validationErrors);

        Assert.Same(operationResult, returned);
        Assert.Equal(2, operationResult.Errors.Count);
        Assert.Same(e1, operationResult.Errors.First());
        Assert.Same(e2, operationResult.Errors.Skip(1).First());
    }

    [Fact]
    public void AddErrors_AppendsToExistingErrors()
    {
        var operationResult = CreateEmptyStringResult();
        var existing = new ErrorResult("existing", 0, "ex0");

        operationResult.Errors.Add(existing);

        var e1 = new ErrorResult("e1", 1, "ex1");
        var e2 = new ErrorResult("e2", 2, "ex2");
        var validationErrors = new List<ErrorResult> { e1, e2 };

        operationResult.AddErrors(validationErrors);

        Assert.Equal(3, operationResult.Errors.Count);
        Assert.Same(existing, operationResult.Errors.First());
        Assert.Same(e1, operationResult.Errors.Skip(1).First());
        Assert.Same(e2, operationResult.Errors.Skip(2).First());
    }

    [Fact]
    public void AddErrors_AllowsNullEntriesInValidationErrors()
    {
        var operationResult = CreateEmptyStringResult();
        var e1 = new ErrorResult("e1", 1, "ex1");
        ErrorResult? eNull = null;
        var validationErrors = new List<ErrorResult?> { e1, eNull };

        // cast explícito a IEnumerable<ErrorResult> para coincidir con la firma del método
        operationResult.AddErrors(validationErrors!);

        Assert.Equal(2, operationResult.Errors.Count);
        Assert.Same(e1, operationResult.Errors.First());
        Assert.Null(operationResult.Errors.Skip(1).First());
    }
    #endregion
}