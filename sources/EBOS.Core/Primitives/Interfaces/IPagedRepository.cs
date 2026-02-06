namespace EBOS.Core.Primitives.Interfaces;

public interface IPagedRepository<T> : IRepository<T> where T : class
{
    Task<IReadOnlyCollection<T>> GetAllPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}