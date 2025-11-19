using EBOS.Core.BusinessErrors;

namespace EBOS.Core.Test.BusinessErrors;

public class BusinessErrorObjectTests
{
    #region Default state
    [Fact]
    public void NewInstance_HasDefaultValues()
    {
        var error = new BusinessErrorObject();

        // En tiempo de ejecución, al usar "= null!" el valor inicial es realmente null
        Assert.Null(error.Message);
        Assert.Equal(0, error.Code);
    }
    #endregion

    #region Message property
    [Fact]
    public void Message_CanBeSetAndRetrieved()
    {
        var error = new BusinessErrorObject
        {
            Message = "A business error occurred"
        };

        Assert.Equal("A business error occurred", error.Message);
    }

    [Fact]
    public void Message_CanBeSetToEmptyString()
    {
        var error = new BusinessErrorObject
        {
            Message = string.Empty
        };

        Assert.Equal(string.Empty, error.Message);
    }

    [Fact]
    public void Message_CanBeSetToNull()
    {
        var error = new BusinessErrorObject
        {
            Message = null!
        };

        Assert.Null(error.Message);
    }
    #endregion

    #region Code property
    [Fact]
    public void Code_CanBeSetAndRetrieved()
    {
        var error = new BusinessErrorObject
        {
            Code = 1001
        };

        Assert.Equal(1001, error.Code);
    }

    [Fact]
    public void Code_CanBeZero()
    {
        var error = new BusinessErrorObject
        {
            Code = 0
        };

        Assert.Equal(0, error.Code);
    }

    [Fact]
    public void Code_CanBeNegative()
    {
        var error = new BusinessErrorObject
        {
            Code = -5
        };

        Assert.Equal(-5, error.Code);
    }
    #endregion

    #region Combined scenarios
    [Fact]
    public void Properties_CanBeSetIndependently()
    {
        var error = new BusinessErrorObject
        {
            Message = "Invalid operation",
            Code = 400
        };

        Assert.Equal("Invalid operation", error.Message);
        Assert.Equal(400, error.Code);

        error.Message = "Updated message";
        error.Code = 401;

        Assert.Equal("Updated message", error.Message);
        Assert.Equal(401, error.Code);
    }
    #endregion
}