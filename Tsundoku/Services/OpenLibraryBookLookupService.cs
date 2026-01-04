using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace Tsundoku.Services;

public sealed class OpenLibraryBookLookupService(HttpClient httpClient) : IBookLookupService
{
	private sealed class SearchResponse
	{
		[JsonPropertyName("docs")]
		public List<SearchDoc> Docs { get; set; } = [];
	}

	private sealed class SearchDoc
	{
		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("author_name")]
		public List<string>? AuthorNames { get; set; }

		[JsonPropertyName("first_sentence")]
		public object? FirstSentence { get; set; }

		[JsonPropertyName("cover_i")]
		public int? CoverId { get; set; }
	}

	public async Task<BookLookupResult?> LookupByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
	{
		var normalized = NormalizeIsbn(isbn);
		if (string.IsNullOrWhiteSpace(normalized))
			return null;

		var url = $"https://openlibrary.org/search.json?isbn={Uri.EscapeDataString(normalized)}";
		SearchResponse? response;
		try
		{
			response = await httpClient.GetFromJsonAsync<SearchResponse>(url, cancellationToken).ConfigureAwait(false);
		}
		catch
		{
			return null;
		}
		var doc = response?.Docs?.FirstOrDefault();
		if (doc is null)
			return null;

		var author = doc.AuthorNames?.FirstOrDefault();
		return new BookLookupResult
		{
			Title = doc.Title,
			Author = author,
			ShortDescription = ExtractFirstSentence(doc.FirstSentence),
			CoverImageUrl = doc.CoverId is null ? null : $"https://covers.openlibrary.org/b/id/{doc.CoverId}-L.jpg?default=false",
		};
	}

	private static string? ExtractFirstSentence(object? firstSentence)
	{
		return firstSentence switch
		{
			JsonElement je when je.ValueKind == JsonValueKind.String => je.GetString(),
			JsonElement je when je.ValueKind == JsonValueKind.Object && je.TryGetProperty("value", out var v) && v.ValueKind == JsonValueKind.String => v.GetString(),
			string s => s,
			Dictionary<string, object> dict when dict.TryGetValue("value", out var v) => v?.ToString(),
			_ => null,
		};
	}

	private static string NormalizeIsbn(string isbn)
	{
		var chars = isbn.Where(char.IsLetterOrDigit).ToArray();
		return new string(chars);
	}
}
