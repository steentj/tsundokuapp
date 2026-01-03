using Tsundoku.Models;

namespace Tsundoku.Services;

public interface IBookRepository
{
	Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
	Task<int> SaveAsync(Book book, CancellationToken cancellationToken = default);
	Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
