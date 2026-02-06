using EBOS.Core.Primitives;
using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives;

public class ErrorResultTests
{
    [Fact]
    public void Constructor_AssignsProperties()
    {
        var err = new ErrorResult("msg", 7, "ex");
        Assert.Equal(7, err.Code);
        Assert.Equal("msg", err.Message);
        Assert.Equal("ex", err.ExceptionMsg);
    }

    [Fact]
    public void Constructor_ThrowsOnNullMessage()
    {
        Assert.Throws<ArgumentNullException>(() => new ErrorResult(null!, 1));
    }
}