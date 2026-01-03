using Tsundoku.Data;
using Tsundoku.Models;

namespace Tsundoku.Services;

public sealed class BookRepository(TsundokuDb db) : IBookRepository
{
	private async Task EnsureInitAsync()
	{
		await db.InitializeAsync().ConfigureAwait(false);
	}

	public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		await EnsureInitAsync().ConfigureAwait(false);
		return await db.Connection.Table<Book>()
			.OrderByDescending(b => b.DateBought)
			.ThenBy(b => b.Title)
			.ToListAsync()
			.ConfigureAwait(false);
	}

	public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		await EnsureInitAsync().ConfigureAwait(false);
		return await db.Connection.Table<Book>()
			.Where(b => b.Id == id)
			.FirstOrDefaultAsync()
			.ConfigureAwait(false);
	}

	public async Task<int> SaveAsync(Book book, CancellationToken cancellationToken = default)
	{
		await EnsureInitAsync().ConfigureAwait(false);
		book.UpdatedAtUtc = DateTime.UtcNow;

		if (book.Id == 0)
			return await db.Connection.InsertAsync(book).ConfigureAwait(false);

		return await db.Connection.UpdateAsync(book).ConfigureAwait(false);
	}

	public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		await EnsureInitAsync().ConfigureAwait(false);
		await db.Connection.DeleteAsync<Book>(id).ConfigureAwait(false);
	}
}
