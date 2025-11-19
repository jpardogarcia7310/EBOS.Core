using EBOS.Core.Primitives;

namespace EBOS.Core.Test;

public class OperationResultTests
{
    #region Constructor
    [Fact]
    public void Constructor_InitializesErrors_AsEmptyCollection()
    {
        var result = new OperationResult<string>();

        Assert.NotNull(result.Errors);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Constructor_HasErrors_IsFalse_WhenNoErrors()
    {
        var result = new OperationResult<string>();

        Assert.False(result.HasErrors);
    }
    #endregion

    #region Result property
    [Fact]
    public void Result_CanBeSetAndRead_ForReferenceType()
    {
        var operationResult = new OperationResult<string>
        {
            Result = "OK"
        };

        Assert.Equal("OK", operationResult.Result);
    }

    [Fact]
    public void Result_DefaultValue_ForReferenceType_IsNullAtRuntime()
    {
        var operationResult = new OperationResult<string>();

        Assert.Null(operationResult.Result);
    }

    [Fact]
    public void Result_CanBeSetAndRead_ForValueType()
    {
        var operationResult = new OperationResult<int>
        {
            Result = 42
        };

        Assert.Equal(42, operationResult.Result);
    }

    [Fact]
    public void Result_DefaultValue_ForValueType_IsZero()
    {
        var operationResult = new OperationResult<int>();

        Assert.Equal(0, operationResult.Result);
    }
    #endregion

    #region Errors collection
    [Fact]
    public void Errors_IsSameInstanceOnMultipleAccess()
    {
        var operationResult = new OperationResult<string>();

        ICollection<ErrorResult> first = operationResult.Errors;
        ICollection<ErrorResult> second = operationResult.Errors;

        Assert.Same(first, second);
    }

    [Fact]
    public void Errors_CanAddSingleError()
    {
        var operationResult = new OperationResult<string>();
        var error = new ErrorResult("Error message", 1, "ex");

        operationResult.Errors.Add(error);

        Assert.Single(operationResult.Errors);
        Assert.Same(error, operationResult.Errors.First());
    }

    [Fact]
    public void Errors_CanAddMultipleErrors()
    {
        var operationResult = new OperationResult<string>();
        var e1 = new ErrorResult("First", 1, "ex1");
        var e2 = new ErrorResult("Second", 2, "ex2");

        operationResult.Errors.Add(e1);
        operationResult.Errors.Add(e2);

        Assert.Equal(2, operationResult.Errors.Count);
        Assert.Contains(e1, operationResult.Errors);
        Assert.Contains(e2, operationResult.Errors);
    }

    [Fact]
    public void Errors_AllowsAddingNullEntries()
    {
        var operationResult = new OperationResult<string>();

        operationResult.Errors.Add(null!);

        Assert.Single(operationResult.Errors);
        Assert.Null(operationResult.Errors.First());
    }

    [Fact]
    public void Errors_Instance_IsWritableButPropertyIsReadOnly()
    {
        var operationResult = new OperationResult<string>();

        operationResult.Errors.Add(new ErrorResult("e", 1, "ex"));

        Assert.Single(operationResult.Errors);
    }
    #endregion

    #region HasErrors
    [Fact]
    public void HasErrors_ReturnsFalse_WhenNoErrors()
    {
        var operationResult = new OperationResult<string>();

        Assert.False(operationResult.HasErrors);
    }

    [Fact]
    public void HasErrors_ReturnsTrue_WhenSingleErrorAdded()
    {
        var operationResult = new OperationResult<string>();

        operationResult.Errors.Add(new ErrorResult("e", 1, "ex"));

        Assert.True(operationResult.HasErrors);
    }

    [Fact]
    public void HasErrors_ReturnsTrue_WhenMultipleErrorsAdded()
    {
        var operationResult = new OperationResult<string>();

        operationResult.Errors.Add(new ErrorResult("e1", 1, "ex1"));
        operationResult.Errors.Add(new ErrorResult("e2", 2, "ex2"));

        Assert.True(operationResult.HasErrors);
    }

    [Fact]
    public void HasErrors_ReflectsRemovalOfErrors()
    {
        var operationResult = new OperationResult<string>();
        var error = new ErrorResult("e", 1, "ex");

        operationResult.Errors.Add(error);

        Assert.True(operationResult.HasErrors);

        operationResult.Errors.Remove(error);

        Assert.False(operationResult.HasErrors);
    }

    [Fact]
    public void HasErrors_True_WhenNullErrorAdded()
    {
        var operationResult = new OperationResult<string>();

        operationResult.Errors.Add(null!);

        Assert.True(operationResult.HasErrors);
    }
    #endregion

    #region Different TResult types
    [Fact]
    public void OperationResult_WithComplexType_WorksCorrectly()
    {
        var operationResult = new OperationResult<DummyResult>();
        var dummy = new DummyResult { Id = 10, Name = "Test" };

        operationResult.Result = dummy;
        operationResult.Errors.Add(new ErrorResult("e", 1, "ex"));

        Assert.Same(dummy, operationResult.Result);
        Assert.True(operationResult.HasErrors);
    }

    private class DummyResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    #endregion
}