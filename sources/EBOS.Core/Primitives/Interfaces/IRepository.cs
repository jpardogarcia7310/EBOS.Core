namespace EBOS.Core.Primitives.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    #region Commands
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    void Update(TEntity entity);
    void DeletreAsync(TEntity entity, CancellationToken cancellationToken);
    #endregion

    #region Queries
    Task<TEntity?> GetByIdAsync(long id);
    Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    #endregion
}