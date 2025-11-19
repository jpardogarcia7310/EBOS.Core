using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.Core.Test.Primitives;

public class BaseEntityTests
{
    #region Helper concrete implementation
    private class TestEntity : BaseEntity
    {
        // No extra members needed for testing BaseEntity
    }
    #endregion

    #region Id property behavior
    [Fact]
    public void NewEntity_HasDefaultIdZero()
    {
        var entity = new TestEntity();

        Assert.Equal(0L, entity.Id);
    }

    [Fact]
    public void CanSetAndGetId()
    {
        var entity = new TestEntity
        {
            Id = 123L
        };

        Assert.Equal(123L, entity.Id);
    }
    #endregion

    #region Attributes on Id property
    [Fact]
    public void Id_HasKeyAttribute()
    {
        var prop = typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id));

        Assert.NotNull(prop);

        var keyAttr = prop!.GetCustomAttributes(typeof(KeyAttribute), inherit: true);

        Assert.NotEmpty(keyAttr);
    }

    [Fact]
    public void Id_HasDatabaseGeneratedAttribute_WithIdentityOption()
    {
        var prop = typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id));

        Assert.NotNull(prop);

        var dbGeneratedAttrs = prop!.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), inherit: true);

        Assert.NotEmpty(dbGeneratedAttrs);

        var dbGeneratedAttr = Assert.IsType<DatabaseGeneratedAttribute>(dbGeneratedAttrs[0]);

        Assert.Equal(DatabaseGeneratedOption.Identity, dbGeneratedAttr.DatabaseGeneratedOption);
    }

    [Fact]
    public void Id_IsOfTypeLong()
    {
        var prop = typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id));

        Assert.NotNull(prop);
        Assert.Equal(typeof(long), prop!.PropertyType);
    }
    #endregion
}