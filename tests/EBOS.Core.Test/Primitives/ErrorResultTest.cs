using EBOS.Core.Primitives;
using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives;

public class ErrorResultTests
    {
        #region Interface contract

        [Fact]
        public void ErrorResult_ImplementsIErrorResult()
        {
            var error = new ErrorResult("msg", 1, "ex");

            Assert.IsAssignableFrom<IErrorResult>(error);
        }

        #endregion

        #region Constructor behavior

        [Fact]
        public void Constructor_SetsAllProperties()
        {
            var error = new ErrorResult("An error occurred", 100, "Exception details");

            Assert.Equal("An error occurred", error.Message);
            Assert.Equal(100, error.Code);
            Assert.Equal("Exception details", error.ExceptionMsg);
        }

        [Fact]
        public void Constructor_AllowsNullExceptionMsg()
        {
            var error = new ErrorResult("Error without exception", 200, null);

            Assert.Equal("Error without exception", error.Message);
            Assert.Equal(200, error.Code);
            Assert.Null(error.ExceptionMsg);
    }

    [Fact]
    public void Constructor_AllowsEmptyMessage()
    {
        var error = new ErrorResult(string.Empty, 0, null);

        Assert.Equal(string.Empty, error.Message);
        Assert.Equal(0, error.Code);
        Assert.Null(error.ExceptionMsg);
    }

    [Fact]
    public void Constructor_AllowsNegativeCode()
    {
        var error = new ErrorResult("Negative code", -1, null);

        Assert.Equal("Negative code", error.Message);
        Assert.Equal(-1, error.Code);
        Assert.Null(error.ExceptionMsg);
    }

    [Fact]
    public void Constructor_AllowsNullMessageAtRuntime()
    {
        string? message = null;

        var error = new ErrorResult(message!, 10, "ex");

        Assert.Null(error.Message);
        Assert.Equal(10, error.Code);
        Assert.Equal("ex", error.ExceptionMsg);
    }

    #endregion

    #region Property setters

    [Fact]
    public void Message_PropertyIsSettable()
    {
        var error = new ErrorResult("initial", 1, null);

        error.Message = "updated";

        Assert.Equal("updated", error.Message);
    }

    [Fact]
    public void Code_PropertyIsSettable()
    {
        var error = new ErrorResult("msg", 1, null);

        error.Code = 999;

        Assert.Equal(999, error.Code);
    }

    [Fact]
    public void ExceptionMsg_PropertyIsSettableToNonNull()
    {
        var error = new ErrorResult("msg", 1, null);

        error.ExceptionMsg = "new exception details";

        Assert.Equal("new exception details", error.ExceptionMsg);
    }

    [Fact]
    public void ExceptionMsg_PropertyIsSettableToNull()
    {
        var error = new ErrorResult("msg", 1, "ex");

        error.ExceptionMsg = null;

        Assert.Null(error.ExceptionMsg);
    }

    #endregion

    #region Combined scenarios

    [Fact]
    public void Properties_CanBeMutatedIndependently()
    {
        var error = new ErrorResult("initial", 100, "ex1");

        error.Message = "changed";
        error.Code = 200;
        error.ExceptionMsg = "ex2";

        Assert.Equal("changed", error.Message);
        Assert.Equal(200, error.Code);
        Assert.Equal("ex2", error.ExceptionMsg);
    }

    #endregion
}
