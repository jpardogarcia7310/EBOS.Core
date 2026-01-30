using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives.Interfaces;

public class IRepositoryTests
{
    #region Helper implementation
    private class TestRepository<TEntity> : IRepository<TEntity> where TEntity : class
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
    }
    #endregion

    #region Interface shape (reflection)
    [Fact]
    public void IRepository_HasMethod_AddAsync()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod(nameof(IRepository<object>.AddAsync));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method.ReturnType);

        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);

        Assert.True(parameters[0].ParameterType.IsGenericParameter);
        Assert.Equal("TEntity", parameters[0].ParameterType.Name);

        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
    }

    [Fact]
    public void IRepository_HasMethod_AddRangeAsync()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod(nameof(IRepository<object>.AddRangeAsync));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method.ReturnType);

        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);

        Assert.True(parameters[0].ParameterType.IsGenericType);
        Assert.Equal(typeof(IEnumerable<>), parameters[0].ParameterType.GetGenericTypeDefinition());
        Assert.True(parameters[0].ParameterType.GenericTypeArguments[0].IsGenericParameter);

        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
    }

    [Fact]
    public void IRepository_HasMethod_AttachAsync()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod(nameof(IRepository<object>.AttachAsync));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method.ReturnType);

        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);

        Assert.True(parameters[0].ParameterType.IsGenericParameter);
        Assert.Equal("TEntity", parameters[0].ParameterType.Name);

        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
    }

    [Fact]
    public void IRepository_HasMethod_UpdateAsync()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod(nameof(IRepository<object>.UpdateAsync));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method.ReturnType);

        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);

        Assert.True(parameters[0].ParameterType.IsGenericParameter);
        Assert.Equal("TEntity", parameters[0].ParameterType.Name);

        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
    }

    [Fact]
    public void IRepository_HasMethod_DeleteAsync()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod(nameof(IRepository<object>.DeleteAsync));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method.ReturnType);

        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);

        Assert.True(parameters[0].ParameterType.IsGenericParameter);
        Assert.Equal("TEntity", parameters[0].ParameterType.Name);

        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
    }

    [Fact]
    public void IRepository_HasMethod_GetAllAsync()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod(nameof(IRepository<object>.GetAllAsync));

        Assert.NotNull(method);

        Assert.True(method.ReturnType.IsGenericType);
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());

        var inner = method.ReturnType.GenericTypeArguments[0];
        Assert.Equal(typeof(ICollection<>), inner.GetGenericTypeDefinition());
        Assert.True(inner.GenericTypeArguments[0].IsGenericParameter);

        var parameters = method.GetParameters();
        Assert.Single(parameters);
        Assert.Equal(typeof(CancellationToken), parameters[0].ParameterType);
    }

    [Fact]
    public void IRepository_HasMethod_GetByIdAsync()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod(nameof(IRepository<object>.GetByIdAsync));

        Assert.NotNull(method);

        Assert.True(method.ReturnType.IsGenericType);
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());

        var inner = method.ReturnType.GenericTypeArguments[0];
        Assert.True(inner.IsGenericParameter);

        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);

        Assert.Equal(typeof(long), parameters[0].ParameterType);
        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);
    }
    #endregion

    #region Basic contract behavior via test implementation
    private class DummyEntity { }

    [Fact]
    public async Task Implementation_AllowsCallingAddAsync()
    {
        IRepository<DummyEntity> repo = new TestRepository<DummyEntity>();
        await repo.AddAsync(new DummyEntity(), CancellationToken.None);
    }

    [Fact]
    public async Task Implementation_AllowsCallingAddRangeAsync()
    {
        IRepository<DummyEntity> repo = new TestRepository<DummyEntity>();
        await repo.AddRangeAsync([new DummyEntity()], CancellationToken.None);
    }

    [Fact]
    public void Implementation_AllowsCallingAttachAsync()
    {
        IRepository<DummyEntity> repo = new TestRepository<DummyEntity>();
        repo.AttachAsync(new DummyEntity(), CancellationToken.None);
    }

    [Fact]
    public void Implementation_AllowsCallingUpdateAsync()
    {
        IRepository<DummyEntity> repo = new TestRepository<DummyEntity>();
        repo.UpdateAsync(new DummyEntity(), CancellationToken.None);
    }

    [Fact]
    public void Implementation_AllowsCallingDeleteAsync()
    {
        IRepository<DummyEntity> repo = new TestRepository<DummyEntity>();
        repo.DeleteAsync(new DummyEntity(), CancellationToken.None);
    }

    [Fact]
    public async Task Implementation_AllowsCallingGetAllAsync()
    {
        IRepository<DummyEntity> repo = new TestRepository<DummyEntity>();
        var result = await repo.GetAllAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<ICollection<DummyEntity>>(result);
    }

    [Fact]
    public async Task Implementation_AllowsCallingGetByIdAsync()
    {
        IRepository<DummyEntity> repo = new TestRepository<DummyEntity>();
        var result = await repo.GetByIdAsync(1, CancellationToken.None);

        Assert.Null(result);
    }
    #endregion
}
