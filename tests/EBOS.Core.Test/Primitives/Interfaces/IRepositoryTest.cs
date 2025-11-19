using EBOS.Core.Primitives.Interfaces;

namespace EBOS.Core.Test.Primitives.Interfaces;

public class IRepositoryTests
{
    #region Helper types
    private class TestEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class InMemoryRepository : IRepository<TestEntity>
    {
        private readonly List<TestEntity> _entities = [];
        public int SaveChangesCallCount { get; private set; }
        public CancellationToken? LastSaveChangesToken { get; private set; }

        public Task AddAsync(TestEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            _entities.Add(entity);

            return Task.CompletedTask;
        }

        public void Update(TestEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var existing = _entities.FirstOrDefault(e => e.Id == entity.Id);
            if (existing == null)
                return;

            // Asignaciones claras, en sentencias separadas
            existing.Id = entity.Id;
            existing.Name = entity.Name;
        }

        public Task<TestEntity?> GetByIdAsync(long id)
        {
            var entity = _entities.FirstOrDefault(e => e.Id == id);

            return Task.FromResult(entity);
        }

        public Task<IList<TestEntity>> GetAllAsync()
        {
            // Devolver copia para evitar modificaciones externas
            IList<TestEntity> copy = [.. _entities];

            return Task.FromResult(copy);
        }

        public Task SaveChangesAsync(CancellationToken token = default)
        {
            SaveChangesCallCount++;
            LastSaveChangesToken = token;
            return Task.CompletedTask;
        }
    }
    #endregion

    #region Interface shape via reflection
    [Fact]
    public void IRepository_HasAddAsyncMethod_WithCorrectSignature()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod("AddAsync");

        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method!.ReturnType);

        var parameters = method.GetParameters();

        Assert.Single(parameters);
        Assert.Equal("entity", parameters[0].Name);
    }

    [Fact]
    public void IRepository_HasUpdateMethod_WithCorrectSignature()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod("Update");

        Assert.NotNull(method);
        Assert.Equal(typeof(void), method!.ReturnType);

        var parameters = method.GetParameters();

        Assert.Single(parameters);
        Assert.Equal("entity", parameters[0].Name);
    }

    [Fact]
    public void IRepository_HasGetByIdAsyncMethod_WithCorrectSignature()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod("GetByIdAsync");

        Assert.NotNull(method);
        // Task<T?>
        Assert.True(method!.ReturnType.IsGenericType);
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());

        var parameters = method.GetParameters();

        Assert.Single(parameters);
        Assert.Equal(typeof(long), parameters[0].ParameterType);
        Assert.Equal("id", parameters[0].Name);
    }

    [Fact]
    public void IRepository_HasGetAllAsyncMethod_WithCorrectSignature()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod("GetAllAsync");

        Assert.NotNull(method);
        // Task<IList<T>>
        Assert.True(method!.ReturnType.IsGenericType);
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());

        var resultArg = method.ReturnType.GetGenericArguments()[0];

        Assert.True(resultArg.IsGenericType);
        Assert.Equal(typeof(IList<>), resultArg.GetGenericTypeDefinition());
    }

    [Fact]
    public void IRepository_HasSaveChangesAsyncMethod_WithCorrectSignature()
    {
        var type = typeof(IRepository<>);
        var method = type.GetMethod("SaveChangesAsync");

        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method!.ReturnType);

        var parameters = method.GetParameters();

        Assert.Single(parameters);
        Assert.Equal(typeof(CancellationToken), parameters[0].ParameterType);
        Assert.True(parameters[0].IsOptional);
        Assert.Equal("token", parameters[0].Name);
    }

    #endregion

    #region Behavioral tests using InMemoryRepository

    [Fact]
    public async Task AddAsync_AddsEntityToRepository()
    {
        var repo = new InMemoryRepository();
        var entity = new TestEntity { Id = 1, Name = "Test" };

        await repo.AddAsync(entity);

        var all = await repo.GetAllAsync();

        Assert.Single(all);
        Assert.Equal(1, all[0].Id);
        Assert.Equal("Test", all[0].Name);
    }

    [Fact]
    public async Task AddAsync_NullEntity_ThrowsArgumentNullException()
    {
        var repo = new InMemoryRepository();

        await Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddAsync(null!));
    }

    [Fact]
    public async Task GetAllAsync_OnEmptyRepository_ReturnsEmptyList()
    {
        var repo = new InMemoryRepository();
        var all = await repo.GetAllAsync();

        Assert.NotNull(all);
        Assert.Empty(all);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
    {
        var repo = new InMemoryRepository();
        var entity = new TestEntity { Id = 10, Name = "Entity10" };

        await repo.AddAsync(entity);

        var result = await repo.GetByIdAsync(10);

        Assert.NotNull(result);
        Assert.Equal(10, result!.Id);
        Assert.Equal("Entity10", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingEntity_ReturnsNull()
    {
        var repo = new InMemoryRepository();

        await repo.AddAsync(new TestEntity { Id = 1, Name = "One" });

        var result = await repo.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_ExistingEntity_UpdatesStoredEntity()
    {
        var repo = new InMemoryRepository();

        await repo.AddAsync(new TestEntity { Id = 5, Name = "Old" });
        repo.Update(new TestEntity { Id = 5, Name = "New" });

        var result = await repo.GetByIdAsync(5);

        Assert.NotNull(result);
        Assert.Equal("New", result!.Name);
    }

    [Fact]
    public async Task Update_NonExistingEntity_DoesNothing()
    {
        var repo = new InMemoryRepository();

        await repo.AddAsync(new TestEntity { Id = 1, Name = "One" });
        repo.Update(new TestEntity { Id = 2, Name = "Two" });

        var all = await repo.GetAllAsync();

        Assert.Single(all);
        Assert.Equal(1, all[0].Id);
        Assert.Equal("One", all[0].Name);
    }

    [Fact]
    public void Update_NullEntity_ThrowsArgumentNullException()
    {
        var repo = new InMemoryRepository();

        Assert.Throws<ArgumentNullException>(() => repo.Update(null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCopy_NotSameReferenceAsInternalList()
    {
        var repo = new InMemoryRepository();

        await repo.AddAsync(new TestEntity { Id = 1, Name = "One" });

        var list1 = await repo.GetAllAsync();
        var list2 = await repo.GetAllAsync();

        Assert.NotSame(list1, list2);
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutToken_IncrementsCallCount()
    {
        var repo = new InMemoryRepository();

        await repo.SaveChangesAsync();

        Assert.Equal(1, repo.SaveChangesCallCount);
        Assert.True(repo.LastSaveChangesToken.HasValue);
        Assert.Equal(default, repo.LastSaveChangesToken.Value);
    }

    [Fact]
    public async Task SaveChangesAsync_WithCancellationToken_PassesToken()
    {
        var repo = new InMemoryRepository();
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        await repo.SaveChangesAsync(token);

        Assert.Equal(1, repo.SaveChangesCallCount);
        Assert.True(repo.LastSaveChangesToken.HasValue);
        Assert.Equal(token, repo.LastSaveChangesToken.Value);
    }

    [Fact]
    public async Task SaveChangesAsync_CanBeCalledMultipleTimes_IncrementsCallCount()
    {
        var repo = new InMemoryRepository();

        await repo.SaveChangesAsync();
        await repo.SaveChangesAsync();

        Assert.Equal(2, repo.SaveChangesCallCount);
    }
    #endregion
}