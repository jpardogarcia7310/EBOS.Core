namespace EBOS.Core.Primitives.Interfaces;

public interface IReadOnlyRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}