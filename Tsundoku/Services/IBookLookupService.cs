namespace Tsundoku.Services;

public interface IBookLookupService
{
	Task<BookLookupResult?> LookupByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
}
