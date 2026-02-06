using EBOS.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOS.Core.Test.Primitives;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity { }

    // ----- INSTANTIATION -----

    [Fact]
    public void BaseEntity_ShouldBeInstantiableThroughChildClass()
    {
        var entity = new TestEntity();

        Assert.NotNull(entity);
    }

    // ----- ID DEFAULT -----

    [Fact]
    public void Id_DefaultValue_ShouldBeZero()
    {
        var entity = new TestEntity();

        Assert.Equal(0L, entity.Id);
    }

    // ----- ID WRITE/READ -----

    [Theory]
    [InlineData(1)]
    [InlineData(9999999999)]
    [InlineData(long.MaxValue)]
    public void Id_ShouldAllowValidAssignments(long value)
    {
        var entity = new TestEntity { Id = value };

        Assert.Equal(value, entity.Id);
    }

    // ----- ATTRIBUTE CHECKS -----

    [Fact]
    public void Id_ShouldHaveKeyAttribute()
    {
        var info = typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id)) ??
            throw new InvalidOperationException("Id property not found on BaseEntity.");
        var keyAttr = info
            .GetCustomAttributes(typeof(KeyAttribute), inherit: true)
            .OfType<KeyAttribute>()
            .SingleOrDefault();

        Assert.NotNull(keyAttr);
    }

    [Fact]
    public void Id_ShouldHaveDatabaseGeneratedIdentityAttribute()
    {
        var info = typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id)) ?? 
            throw new InvalidOperationException("Id property not found on BaseEntity.");
        var attr = info.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true)
                       .OfType<DatabaseGeneratedAttribute>()
                       .FirstOrDefault();

        Assert.NotNull(attr);
        Assert.Equal(DatabaseGeneratedOption.Identity, attr.DatabaseGeneratedOption);
    }

    // ----- TYPE CHECKS -----

    [Fact]
    public void BaseEntity_ShouldBeAbstract()
    {
        Assert.True(typeof(BaseEntity).IsAbstract);
    }

    [Fact]
    public void BaseEntity_ShouldContainOnlyOneProperty_Id()
    {
        var props = typeof(BaseEntity).GetProperties().Select(x => x.Name).ToList();

        Assert.Single(props);
        Assert.Equal("Id", props[0]);
    }
}