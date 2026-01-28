using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives.Interfaces;

public class IUserInfoTests
{
    #region Helper implementation
    private class TestUserInfo : IUserInfo
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long LanguageId { get; set; }
        public string Email { get; set; } = string.Empty;
        public long ConnectionId { get; set; }
    }
    #endregion

    #region Interface shape (reflection)
    [Fact]
    public void IUserInfo_HasProperty_Id_WithGetAndSet_OfTypeLong()
    {
        var type = typeof(IUserInfo);
        var prop = type.GetProperty(nameof(IUserInfo.Id));

        Assert.NotNull(prop);
        Assert.Equal(typeof(long), prop.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }

    [Fact]
    public void IUserInfo_HasProperty_Username_WithGetAndSet_OfTypeString()
    {
        var type = typeof(IUserInfo);
        var prop = type.GetProperty(nameof(IUserInfo.Username));

        Assert.NotNull(prop);
        Assert.Equal(typeof(string), prop.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }

    [Fact]
    public void IUserInfo_HasProperty_Name_WithGetAndSet_OfTypeString()
    {
        var type = typeof(IUserInfo);
        var prop = type.GetProperty(nameof(IUserInfo.Name));

        Assert.NotNull(prop);
        Assert.Equal(typeof(string), prop.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }

    [Fact]
    public void IUserInfo_HasProperty_LanguageId_WithGetAndSet_OfTypeLong()
    {
        var type = typeof(IUserInfo);
        var prop = type.GetProperty(nameof(IUserInfo.LanguageId));

        Assert.NotNull(prop);
        Assert.Equal(typeof(long), prop.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }

    [Fact]
    public void IUserInfo_HasProperty_Email_WithGetAndSet_OfTypeString()
    {
        var type = typeof(IUserInfo);
        var prop = type.GetProperty(nameof(IUserInfo.Email));

        Assert.NotNull(prop);
        Assert.Equal(typeof(string), prop.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }

    [Fact]
    public void IUserInfo_HasProperty_ConnectionId_WithGetAndSet_OfTypeLong()
    {
        var type = typeof(IUserInfo);
        var prop = type.GetProperty(nameof(IUserInfo.ConnectionId));

        Assert.NotNull(prop);
        Assert.Equal(typeof(long), prop.PropertyType);
        Assert.True(prop.CanRead);
        Assert.True(prop.CanWrite);
    }
    #endregion

    #region Basic contract behavior via test implementation
    [Fact]
    public void Implementation_AllowsSetAndGetOfId()
    {
        IUserInfo user = new TestUserInfo
        {
            Id = 42
        };

        Assert.Equal(42, user.Id);
    }

    [Fact]
    public void Implementation_AllowsSetAndGetOfUsername()
    {
        IUserInfo user = new TestUserInfo
        {
            Username = "Jane"
        };

        Assert.Equal("Jane", user.Username);
    }

    [Fact]
    public void Implementation_AllowsSetAndGetOfName()
    {
        IUserInfo user = new TestUserInfo
        {
            Name = "John Doe"
        };

        Assert.Equal("John Doe", user.Name);
    }

    [Fact]
    public void Implementation_AllowsSetAndGetOfLanguageId()
    {
        IUserInfo user = new TestUserInfo
        {
            LanguageId = 7
        };

        Assert.Equal(7, user.LanguageId);
    }

    [Fact]
    public void Implementation_AllowsSetAndGetOfEmail()
    {
        IUserInfo user = new TestUserInfo
        {
            Email = "john@example.com"
        };

        Assert.Equal("john@example.com", user.Email);
    }

    [Fact]
    public void Implementation_AllowsSetAndGetOfConnectionId()
    {
        IUserInfo user = new TestUserInfo
        {
            ConnectionId = 999
        };

        Assert.Equal(999, user.ConnectionId);
    }

    [Fact]
    public void Implementation_CanBeInitializedWithAllProperties()
    {
        IUserInfo user = new TestUserInfo
        {
            Id = 1,
            Username = "admin",
            Name = "Administrator",
            LanguageId = 2,
            Email = "admin@example.com",
            ConnectionId = 1001
        };

        Assert.Equal(1, user.Id);
        Assert.Equal("admin", user.Username);
        Assert.Equal("Administrator", user.Name);
        Assert.Equal(2, user.LanguageId);
        Assert.Equal("admin@example.com", user.Email);
        Assert.Equal(1001, user.ConnectionId);
    }
    #endregion
}
