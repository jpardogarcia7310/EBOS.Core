namespace EBOS.Core.Primitives.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    #region Commands
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    #endregion

    #region Queries
    Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    #endregion
}