using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives.Interfaces;

public class IReadOnlyPagedRepositoryTests
{
    private class TestReadOnlyPagedRepository<TEntity> : IReadOnlyPagedRepository<TEntity> where TEntity : class
    {
        public Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
            => Task.FromResult<TEntity?>(null);

        public Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<TEntity>>(new List<TEntity>());

        public Task<IReadOnlyCollection<TEntity>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<TEntity>>(new List<TEntity>());

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(0);
    }

    private class DummyEntity { }

    [Fact]
    public void IReadOnlyPagedRepository_HasMethod_GetAllPagedAsync()
    {
        var type = typeof(IReadOnlyPagedRepository<>);
        var method = type.GetMethod(nameof(IReadOnlyPagedRepository<object>.GetAllPagedAsync));

        Assert.NotNull(method);
        var parameters = method.GetParameters();
        Assert.Equal(3, parameters.Length);
        Assert.Equal(typeof(int), parameters[0].ParameterType);
        Assert.Equal(typeof(int), parameters[1].ParameterType);
        Assert.Equal(typeof(CancellationToken), parameters[2].ParameterType);
    }

    [Fact]
    public void IReadOnlyPagedRepository_HasMethod_CountAsync()
    {
        var type = typeof(IReadOnlyPagedRepository<>);
        var method = type.GetMethod(nameof(IReadOnlyPagedRepository<object>.CountAsync));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<int>), method.ReturnType);
    }

    [Fact]
    public async Task Implementation_AllowsCallingPagedMethods()
    {
        IReadOnlyPagedRepository<DummyEntity> repo = new TestReadOnlyPagedRepository<DummyEntity>();
        var list = await repo.GetAllPagedAsync(1, 10, CancellationToken.None);
        var count = await repo.CountAsync(CancellationToken.None);

        Assert.NotNull(list);
        Assert.Equal(0, count);
    }
}
