using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives.Interfaces;

public class IReadOnlyRepositoryTests
{
    private class TestReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        public Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
            => Task.FromResult<TEntity?>(null);

        public Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<TEntity>>(new List<TEntity>());
    }

    private class DummyEntity { }

    [Fact]
    public void IReadOnlyRepository_HasMethod_GetByIdAsync()
    {
        var type = typeof(IReadOnlyRepository<>);
        var method = type.GetMethod(nameof(IReadOnlyRepository<object>.GetByIdAsync));

        Assert.NotNull(method);
        Assert.True(method.ReturnType.IsGenericType);
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());

        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(typeof(long), parameters[0].ParameterType);
        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
    }

    [Fact]
    public void IReadOnlyRepository_HasMethod_GetAllAsync()
    {
        var type = typeof(IReadOnlyRepository<>);
        var method = type.GetMethod(nameof(IReadOnlyRepository<object>.GetAllAsync));

        Assert.NotNull(method);
        Assert.True(method.ReturnType.IsGenericType);
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());

        var inner = method.ReturnType.GenericTypeArguments[0];
        Assert.Equal(typeof(IReadOnlyCollection<>), inner.GetGenericTypeDefinition());
        Assert.True(inner.GenericTypeArguments[0].IsGenericParameter);
    }

    [Fact]
    public async Task Implementation_AllowsCallingGetByIdAsync()
    {
        IReadOnlyRepository<DummyEntity> repo = new TestReadOnlyRepository<DummyEntity>();
        var result = await repo.GetByIdAsync(1, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Implementation_AllowsCallingGetAllAsync()
    {
        IReadOnlyRepository<DummyEntity> repo = new TestReadOnlyRepository<DummyEntity>();
        var result = await repo.GetAllAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<IReadOnlyCollection<DummyEntity>>(result);
    }
}
