namespace EBOS.Core.Primitives.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    #region Commands
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);
    void Attach(TEntity entity, CancellationToken cancellationToken);
    void UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    void DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    #endregion

    #region Queries
    Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken);
    #endregion
}
