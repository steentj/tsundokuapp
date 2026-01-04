using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Tsundoku.Services;

namespace TsundokuAppTests.Services;

public class OpenLibraryBookLookupServiceTests
{
	[Fact]
	public async Task LookupByIsbnAsync_WithValidIsbn_ReturnsBookLookupResult()
	{
		// Arrange
		var isbn = "9780262046305";
		var responseJson = """
		{
			"docs": [
				{
					"title": "Test Book Title",
					"author_name": ["Test Author"],
					"cover_i": 12345
				}
			]
		}
		""";

		var mockHandler = CreateMockHttpHandler(HttpStatusCode.OK, responseJson);
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync(isbn);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("Test Book Title", result.Title);
		Assert.Equal("Test Author", result.Author);
		Assert.Equal("https://covers.openlibrary.org/b/id/12345-L.jpg?default=false", result.CoverImageUrl);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WithIsbnContainingHyphens_NormalizesIsbn()
	{
		// Arrange
		var isbn = "978-0-262-04630-5";
		var responseJson = """
		{
			"docs": [
				{
					"title": "Test Book",
					"author_name": ["Author"]
				}
			]
		}
		""";

		var mockHandler = CreateMockHttpHandler(HttpStatusCode.OK, responseJson);
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync(isbn);

		// Assert
		Assert.NotNull(result);
		mockHandler.Protected().Verify(
			"SendAsync",
			Times.Once(),
			ItExpr.Is<HttpRequestMessage>(req =>
				req.RequestUri!.ToString().Contains("isbn=9780262046305")),
			ItExpr.IsAny<CancellationToken>()
		);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WithEmptyIsbn_ReturnsNull()
	{
		// Arrange
		var mockHandler = new Mock<HttpMessageHandler>();
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync("");

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WithWhitespaceIsbn_ReturnsNull()
	{
		// Arrange
		var mockHandler = new Mock<HttpMessageHandler>();
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync("   ");

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WhenNoResultsFound_ReturnsNull()
	{
		// Arrange
		var isbn = "9999999999999";
		var responseJson = """
		{
			"docs": []
		}
		""";

		var mockHandler = CreateMockHttpHandler(HttpStatusCode.OK, responseJson);
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync(isbn);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WhenHttpRequestFails_ReturnsNull()
	{
		// Arrange
		var isbn = "9780262046305";
		var mockHandler = CreateMockHttpHandler(HttpStatusCode.InternalServerError, "");
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync(isbn);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WithMultipleAuthors_ReturnsFirstAuthor()
	{
		// Arrange
		var isbn = "9780262046305";
		var responseJson = """
		{
			"docs": [
				{
					"title": "Test Book",
					"author_name": ["First Author", "Second Author", "Third Author"]
				}
			]
		}
		""";

		var mockHandler = CreateMockHttpHandler(HttpStatusCode.OK, responseJson);
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync(isbn);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("First Author", result.Author);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WithoutAuthor_ReturnsNullAuthor()
	{
		// Arrange
		var isbn = "9780262046305";
		var responseJson = """
		{
			"docs": [
				{
					"title": "Test Book"
				}
			]
		}
		""";

		var mockHandler = CreateMockHttpHandler(HttpStatusCode.OK, responseJson);
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync(isbn);

		// Assert
		Assert.NotNull(result);
		Assert.Null(result.Author);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WithoutCoverId_ReturnsNullCoverImageUrl()
	{
		// Arrange
		var isbn = "9780262046305";
		var responseJson = """
		{
			"docs": [
				{
					"title": "Test Book",
					"author_name": ["Test Author"]
				}
			]
		}
		""";

		var mockHandler = CreateMockHttpHandler(HttpStatusCode.OK, responseJson);
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync(isbn);

		// Assert
		Assert.NotNull(result);
		Assert.Null(result.CoverImageUrl);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WithFirstSentenceAsString_ReturnsShortDescription()
	{
		// Arrange
		var isbn = "9780262046305";
		var responseJson = """
		{
			"docs": [
				{
					"title": "Test Book",
					"author_name": ["Test Author"],
					"first_sentence": "This is the first sentence."
				}
			]
		}
		""";

		var mockHandler = CreateMockHttpHandler(HttpStatusCode.OK, responseJson);
		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync(isbn);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("This is the first sentence.", result.ShortDescription);
	}

	[Fact]
	public async Task LookupByIsbnAsync_WhenTokenAlreadyCanceled_ReturnsNull()
	{
		// Arrange
		var isbn = "9780262046305";
		var cts = new CancellationTokenSource();
		cts.Cancel();
		
		var mockHandler = new Mock<HttpMessageHandler>();
		mockHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent("""{"docs": []}""")
			});

		var httpClient = new HttpClient(mockHandler.Object);
		var service = new OpenLibraryBookLookupService(httpClient);

		// Act
		var result = await service.LookupByIsbnAsync(isbn, cts.Token);

		// Assert
		Assert.Null(result);
	}

	private static Mock<HttpMessageHandler> CreateMockHttpHandler(HttpStatusCode statusCode, string content)
	{
		var mockHandler = new Mock<HttpMessageHandler>();
		mockHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = statusCode,
				Content = new StringContent(content)
			});
		return mockHandler;
	}
}
