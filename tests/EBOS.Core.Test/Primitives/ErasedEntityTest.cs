using EBOS.Core.Primitives;
using EBOS.Core.Primitives.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace EBOS.Core.Test.Primitives;

public class ErasedEntityTests
{
    #region Helper concrete implementation
    private class TestErasedEntity : ErasedEntity
    {
        // Implementación mínima para poder instanciar ErasedEntity
    }
    #endregion

    #region Interface contract
    [Fact]
    public void ErasedEntity_ImplementsISoftDelete()
    {
        var entity = new TestErasedEntity();

        Assert.IsType<ISoftDelete>(entity, exactMatch: false);
    }
    #endregion

    #region Erased property behavior
    [Fact]
    public void NewEntity_DefaultErasedIsFalse()
    {
        var entity = new TestErasedEntity();

        Assert.False(entity.Erased);
    }

    [Fact]
    public void CanSetErasedToTrue()
    {
        var entity = new TestErasedEntity
        {
            Erased = true
        };

        Assert.True(entity.Erased);
    }

    [Fact]
    public void CanToggleErased()
    {
        var entity = new TestErasedEntity
        {
            Erased = true
        };

        Assert.True(entity.Erased);

        entity.Erased = false;

        Assert.False(entity.Erased);
    }
    #endregion

    #region DataAnnotations on Erased
    [Fact]
    public void Erased_HasRequiredAttribute()
    {
        var prop = typeof(ErasedEntity).GetProperty(nameof(ErasedEntity.Erased));

        Assert.NotNull(prop);

        var requiredAttrs = prop!.GetCustomAttributes(typeof(RequiredAttribute), inherit: true);

        Assert.NotEmpty(requiredAttrs);
    }

    [Fact]
    public void Erased_IsOfTypeBool()
    {
        var prop = typeof(ErasedEntity).GetProperty(nameof(ErasedEntity.Erased));

        Assert.NotNull(prop);
        Assert.Equal(typeof(bool), prop!.PropertyType);
    }

    [Fact]
    public void Validation_WithDefaultErased_Passes()
    {
        var entity = new TestErasedEntity(); // Erased = false
        var context = new ValidationContext(entity);
        var results = new System.Collections.Generic.List<ValidationResult>();
        var isValid = Validator.TryValidateObject(entity, context, results, validateAllProperties: true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Validation_WithErasedTrue_Passes()
    {
        var entity = new TestErasedEntity
        {
            Erased = true
        };
        var context = new ValidationContext(entity);
        var results = new System.Collections.Generic.List<ValidationResult>();
        var isValid = Validator.TryValidateObject(entity, context, results, validateAllProperties: true);

        Assert.True(isValid);
        Assert.Empty(results);
    }
    #endregion
}