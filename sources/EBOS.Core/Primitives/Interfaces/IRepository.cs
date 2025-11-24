namespace EBOS.Core.Primitives.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    void Update(TEntity entity);

    Task<TEntity?> GetByIdAsync(long id);
    Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken);
}