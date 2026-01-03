namespace Tsundoku.Services;

public sealed class BookLookupResult
{
	public string? Title { get; init; }
	public string? Author { get; init; }
	public string? ShortDescription { get; init; }
	public string? CoverImageUrl { get; init; }
}
