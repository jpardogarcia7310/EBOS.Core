using EBOS.Core.Extensions;
using EBOS.Core.Primitives;

namespace EBOS.Core.Test.Extensions;

public class OperationResultExtensionsTests
{
    [Fact]
    public void AddResult_ThrowsArgumentNullException_WhenOperationResultIsNull()
    {
        OperationResult<int>? op = null;
        var ex = Assert.Throws<ArgumentNullException>(() => OperationResultExtensions.AddResult(op!, 5));
        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddResult_SetsResult_AndReturnsSameInstance()
    {
        var op = new OperationResult<string>();
        var returned = OperationResultExtensions.AddResult(op, "hello");
        Assert.Same(op, returned);
        Assert.Equal("hello", op.Result);
    }

    [Fact]
    public void AddResultWithError_ThrowsArgumentNullException_WhenOperationResultIsNull()
    {
        OperationResult<int>? op = null;
        var ex = Assert.Throws<ArgumentNullException>(() =>
            OperationResultExtensions.AddResultWithError(op!, 1, "err", 10, "ex"));
        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddResultWithError_AddsErrorAndSetsResult_WhenCalledWithComponents()
    {
        var op = new OperationResult<int>();
        var returned = OperationResultExtensions.AddResultWithError(op, 42, "failure", 99, "stack");
        Assert.Same(op, returned);
        Assert.Equal(42, op.Result);
        Assert.True(op.HasErrors);
        var err = op.Errors.Single();
        Assert.Equal(99, err.Code);
        Assert.Equal("failure", err.Message);
        Assert.Equal("stack", err.ExceptionMsg);
    }

    [Fact]
    public void AddResultWithError_WithErrorResult_AddsProvidedErrorAndSetsResult()
    {
        var op = new OperationResult<string>();
        var error = new ErrorResult("boom", 7, "ex");
        var returned = OperationResultExtensions.AddResultWithError(op, "ok", error);
        Assert.Same(op, returned);
        Assert.Equal("ok", op.Result);
        Assert.True(op.HasErrors);
        var err = op.Errors.Single();
        Assert.Equal(7, err.Code);
        Assert.Equal("boom", err.Message);
        Assert.Equal("ex", err.ExceptionMsg);
    }

    [Fact]
    public void AddResultWithError_WithNullErrorResult_BubblesArgumentNullException()
    {
        var op = new OperationResult<string>();
        ErrorResult? error = null;
        var ex = Assert.Throws<ArgumentNullException>(() => OperationResultExtensions.AddResultWithError(op, "ok", error!));
        // The ArgumentNullException originates from OperationResult.AddError, param name is "error"
        Assert.Equal("error", ex.ParamName);
    }

    [Fact]
    public void AddError_ThrowsArgumentNullException_WhenOperationResultIsNull()
    {
        OperationResult<int>? op = null;
        var ex = Assert.Throws<ArgumentNullException>(() => OperationResultExtensions.AddError(op!, "m", 1, null));
        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddError_ByComponents_AddsErrorAndReturnsSameInstance()
    {
        var op = new OperationResult<object>();
        var returned = OperationResultExtensions.AddError(op, "msg", 11, "ex");
        Assert.Same(op, returned);
        Assert.True(op.HasErrors);
        var e = op.Errors.Single();
        Assert.Equal(11, e.Code);
        Assert.Equal("msg", e.Message);
        Assert.Equal("ex", e.ExceptionMsg);
    }

    [Fact]
    public void AddError_ByObject_AddsErrorAndReturnsSameInstance()
    {
        var op = new OperationResult<object>();
        var error = new ErrorResult("x", 2, null);
        var returned = OperationResultExtensions.AddError(op, error);
        Assert.Same(op, returned);
        Assert.True(op.HasErrors);
        var e = op.Errors.Single();
        Assert.Equal(2, e.Code);
        Assert.Equal("x", e.Message);
        Assert.Null(e.ExceptionMsg);
    }

    [Fact]
    public void AddError_WithNullErrorResult_BubblesArgumentNullException()
    {
        var op = new OperationResult<object>();
        ErrorResult? error = null;
        var ex = Assert.Throws<ArgumentNullException>(() => OperationResultExtensions.AddError(op, error!));
        Assert.Equal("error", ex.ParamName);
    }

    [Fact]
    public void AddErrors_ThrowsArgumentNullException_WhenOperationResultIsNull()
    {
        OperationResult<int>? op = null;
        var ex = Assert.Throws<ArgumentNullException>(() => OperationResultExtensions.AddErrors(op!, []));
        Assert.Equal("operationResult", ex.ParamName);
        Assert.Contains("operationResult cannot be null", ex.Message);
    }

    [Fact]
    public void AddErrors_ReturnsSameInstance_WhenValidationErrorsIsNull_AndDoesNotThrow()
    {
        var op = new OperationResult<int>();
        IEnumerable<ErrorResult>? list = null;
        var returned = OperationResultExtensions.AddErrors(op, list!);
        Assert.Same(op, returned);
        Assert.False(op.HasErrors);
    }

    [Fact]
    public void AddErrors_AddsAllErrors_FromEnumerable()
    {
        var op = new OperationResult<int>();
        var errors = new List<ErrorResult>
            {
                new("a", 1),
                new("b", 2)
            };
        var returned = OperationResultExtensions.AddErrors(op, errors);
        Assert.Same(op, returned);
        Assert.Equal(2, op.Errors.Count);
        Assert.Contains(op.Errors, e => e.Message == "a" && e.Code == 1);
        Assert.Contains(op.Errors, e => e.Message == "b" && e.Code == 2);
    }

    [Fact]
    public void AddErrors_WithEnumerableContainingNull_BubblesArgumentNullException()
    {
        var op = new OperationResult<int>();
        var errors = new List<ErrorResult?> { new("a", 1), null };
        // Cast to IEnumerable<ErrorResult> to match signature but keep a null inside
        var cast = errors.Cast<ErrorResult>();
        var ex = Assert.Throws<ArgumentNullException>(() => OperationResultExtensions.AddErrors(op, cast));
        // The ArgumentNullException originates from OperationResult.AddError when it receives null
        Assert.Equal("error", ex.ParamName);
    }

    [Fact]
    public void Methods_AreChainable()
    {
        var op = new OperationResult<int>();

        // Use extension methods explicitly to avoid ambiguity with instance methods that return void.
        op = OperationResultExtensions.AddResult(op, 100);
        op = OperationResultExtensions.AddError(op, "err", 5, null);
        op = OperationResultExtensions.AddResultWithError(op, 200, new ErrorResult("e2", 6, null));
        op = OperationResultExtensions.AddErrors(op, [new ErrorResult("e3", 7)]);

        Assert.Equal(200, op.Result);
        Assert.Equal(3, op.Errors.Count); // err(5), e2(6), e3(7)
        Assert.Contains(op.Errors, e => e.Code == 5);
        Assert.Contains(op.Errors, e => e.Code == 6);
        Assert.Contains(op.Errors, e => e.Code == 7);
    }
}
