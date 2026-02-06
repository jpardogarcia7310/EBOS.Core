using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives.Interfaces;

public class IPagedRepositoryTests
{
    private class TestPagedRepository<TEntity> : IPagedRepository<TEntity> where TEntity : class
    {
        public Task AddAsync(TEntity entity, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task AttachAsync(TEntity entity, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken)
            => Task.FromResult<ICollection<TEntity>>(new List<TEntity>());

        public Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken)
            => Task.FromResult<TEntity?>(null);

        public Task<IReadOnlyCollection<TEntity>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyCollection<TEntity>>(new List<TEntity>());

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(0);
    }

    private class DummyEntity { }

    [Fact]
    public void IPagedRepository_HasMethod_GetAllPagedAsync()
    {
        var type = typeof(IPagedRepository<>);
        var method = type.GetMethod(nameof(IPagedRepository<object>.GetAllPagedAsync));

        Assert.NotNull(method);
        var parameters = method.GetParameters();
        Assert.Equal(3, parameters.Length);
        Assert.Equal(typeof(int), parameters[0].ParameterType);
        Assert.Equal(typeof(int), parameters[1].ParameterType);
        Assert.Equal(typeof(CancellationToken), parameters[2].ParameterType);
    }

    [Fact]
    public void IPagedRepository_HasMethod_CountAsync()
    {
        var type = typeof(IPagedRepository<>);
        var method = type.GetMethod(nameof(IPagedRepository<object>.CountAsync));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<int>), method.ReturnType);
    }

    [Fact]
    public async Task Implementation_AllowsCallingPagedMethods()
    {
        IPagedRepository<DummyEntity> repo = new TestPagedRepository<DummyEntity>();
        var list = await repo.GetAllPagedAsync(1, 10, CancellationToken.None);
        var count = await repo.CountAsync(CancellationToken.None);

        Assert.NotNull(list);
        Assert.Equal(0, count);
    }
}
