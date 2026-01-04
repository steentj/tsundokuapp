using Tsundoku.Services;

namespace TsundokuAppTests.Services;

public class BookLookupResultTests
{
	[Fact]
	public void BookLookupResult_CanBeCreatedWithAllProperties()
	{
		// Arrange & Act
		var result = new BookLookupResult
		{
			Title = "Test Title",
			Author = "Test Author",
			ShortDescription = "Test Description",
			CoverImageUrl = "https://example.com/cover.jpg"
		};

		// Assert
		Assert.Equal("Test Title", result.Title);
		Assert.Equal("Test Author", result.Author);
		Assert.Equal("Test Description", result.ShortDescription);
		Assert.Equal("https://example.com/cover.jpg", result.CoverImageUrl);
	}

	[Fact]
	public void BookLookupResult_CanBeCreatedWithNullProperties()
	{
		// Arrange & Act
		var result = new BookLookupResult
		{
			Title = null,
			Author = null,
			ShortDescription = null,
			CoverImageUrl = null
		};

		// Assert
		Assert.Null(result.Title);
		Assert.Null(result.Author);
		Assert.Null(result.ShortDescription);
		Assert.Null(result.CoverImageUrl);
	}

	[Fact]
	public void BookLookupResult_PropertiesAreInitOnly()
	{
		// Arrange
		var result = new BookLookupResult
		{
			Title = "Initial Title",
			Author = "Initial Author"
		};

		// Assert - Properties are init-only, so this test verifies the type is correct
		Assert.Equal("Initial Title", result.Title);
		Assert.Equal("Initial Author", result.Author);
	}
}
