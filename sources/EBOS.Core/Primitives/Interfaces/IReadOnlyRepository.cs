namespace EBOS.Core.Primitives.Interfaces;

public interface IReadOnlyRepository<T> where T : class
{
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
}