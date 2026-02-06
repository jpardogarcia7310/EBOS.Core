namespace EBOS.Core.Primitives.Interfaces;

public interface IReadOnlyPagedRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
{
    Task<IReadOnlyCollection<TEntity>> GetAllPagedAsync(int pageNumber, int pageSize, 
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}