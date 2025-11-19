using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives.Interfaces;

public class IErrorResultTests
{
    #region Helper implementation
    private class TestErrorResult : IErrorResult
    {
        public int Code { get; set; }
        public string? ExceptionMsg { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    #endregion

    #region Interface shape (reflection)
    [Fact]
    public void IErrorResult_HasProperty_Code_WithGetAndSet_OfTypeInt()
    {
        var type = typeof(IErrorResult);
        var prop = type.GetProperty(nameof(IErrorResult.Code));

        Assert.NotNull(prop);
        Assert.Equal(typeof(int), prop!.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }

    [Fact]
    public void IErrorResult_HasProperty_ExceptionMsg_WithGetAndSet_OfTypeString()
    {
        var type = typeof(IErrorResult);
        var prop = type.GetProperty(nameof(IErrorResult.ExceptionMsg));

        Assert.NotNull(prop);
        Assert.Equal(typeof(string), prop!.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }

    [Fact]
    public void IErrorResult_HasProperty_Message_WithGetAndSet_OfTypeString()
    {
        var type = typeof(IErrorResult);
        var prop = type.GetProperty(nameof(IErrorResult.Message));

        Assert.NotNull(prop);
        Assert.Equal(typeof(string), prop!.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }
    #endregion

    #region Basic contract behavior via test implementation
    [Fact]
    public void Implementation_AllowsSetAndGetOfCode()
    {
        IErrorResult error = new TestErrorResult
        {
            Code = 123
        };

        Assert.Equal(123, error.Code);
    }

    [Fact]
    public void Implementation_AllowsSetAndGetOfMessage()
    {
        IErrorResult error = new TestErrorResult
        {
            Message = "An error occurred"
        };

        Assert.Equal("An error occurred", error.Message);
    }

    [Fact]
    public void Implementation_AllowsSetAndGetOfExceptionMsg_NonNull()
    {
        IErrorResult error = new TestErrorResult
        {
            ExceptionMsg = "Stack trace or exception text"
        };

        Assert.Equal("Stack trace or exception text", error.ExceptionMsg);
    }

    [Fact]
    public void Implementation_AllowsSetAndGetOfExceptionMsg_Null()
    {
        IErrorResult error = new TestErrorResult
        {
            ExceptionMsg = null
        };

        Assert.Null(error.ExceptionMsg);
    }

    [Fact]
    public void Implementation_CanBeInitializedWithAllProperties()
    {
        IErrorResult error = new TestErrorResult
        {
            Code = 500,
            Message = "Internal error",
            ExceptionMsg = "System.Exception: something went wrong"
        };

        Assert.Equal(500, error.Code);
        Assert.Equal("Internal error", error.Message);
        Assert.Equal("System.Exception: something went wrong", error.ExceptionMsg);
    }
    #endregion
}
