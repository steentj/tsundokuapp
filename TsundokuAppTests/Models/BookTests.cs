using Tsundoku.Models;

namespace TsundokuAppTests.Models;

public class BookTests
{
	[Fact]
	public void Book_DefaultConstructor_SetsDefaultValues()
	{
		// Arrange & Act
		var book = new Book();

		// Assert
		Assert.Equal(0, book.Id);
		Assert.Equal(string.Empty, book.Title);
		Assert.Equal(string.Empty, book.Author);
		Assert.Equal(DateTime.Today, book.DateBought);
		Assert.Equal(string.Empty, book.ReasonForBuying);
		Assert.Equal(BookStatus.Bought, book.Status);
		Assert.Null(book.ImagePath);
		Assert.Null(book.Isbn);
		Assert.Null(book.ShortDescription);
		Assert.Null(book.DateStartedReading);
		Assert.Null(book.DateFinishedReading);
		Assert.Null(book.Review);
		Assert.Null(book.Rating);
	}

	[Fact]
	public void Book_UpdatedAtUtc_IsSetToUtcNow()
	{
		// Arrange
		var beforeCreation = DateTime.UtcNow.AddSeconds(-1);
		
		// Act
		var book = new Book();
		var afterCreation = DateTime.UtcNow.AddSeconds(1);

		// Assert
		Assert.True(book.UpdatedAtUtc >= beforeCreation);
		Assert.True(book.UpdatedAtUtc <= afterCreation);
	}

	[Fact]
	public void Book_Title_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedTitle = "Test Book Title";

		// Act
		book.Title = expectedTitle;

		// Assert
		Assert.Equal(expectedTitle, book.Title);
	}

	[Fact]
	public void Book_Author_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedAuthor = "Test Author";

		// Act
		book.Author = expectedAuthor;

		// Assert
		Assert.Equal(expectedAuthor, book.Author);
	}

	[Fact]
	public void Book_DateBought_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedDate = new DateTime(2023, 1, 15);

		// Act
		book.DateBought = expectedDate;

		// Assert
		Assert.Equal(expectedDate, book.DateBought);
	}

	[Fact]
	public void Book_ReasonForBuying_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedReason = "Recommended by a friend";

		// Act
		book.ReasonForBuying = expectedReason;

		// Assert
		Assert.Equal(expectedReason, book.ReasonForBuying);
	}

	[Fact]
	public void Book_Status_CanBeSet()
	{
		// Arrange
		var book = new Book();

		// Act
		book.Status = BookStatus.Read;

		// Assert
		Assert.Equal(BookStatus.Read, book.Status);
	}

	[Fact]
	public void Book_ImagePath_CanBeSetToNull()
	{
		// Arrange
		var book = new Book { ImagePath = "/some/path.jpg" };

		// Act
		book.ImagePath = null;

		// Assert
		Assert.Null(book.ImagePath);
	}

	[Fact]
	public void Book_Isbn_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedIsbn = "978-0-123456-78-9";

		// Act
		book.Isbn = expectedIsbn;

		// Assert
		Assert.Equal(expectedIsbn, book.Isbn);
	}

	[Fact]
	public void Book_ShortDescription_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedDescription = "A fascinating tale of adventure";

		// Act
		book.ShortDescription = expectedDescription;

		// Assert
		Assert.Equal(expectedDescription, book.ShortDescription);
	}

	[Fact]
	public void Book_DateStartedReading_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedDate = new DateTime(2023, 2, 1);

		// Act
		book.DateStartedReading = expectedDate;

		// Assert
		Assert.Equal(expectedDate, book.DateStartedReading);
	}

	[Fact]
	public void Book_DateFinishedReading_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedDate = new DateTime(2023, 3, 1);

		// Act
		book.DateFinishedReading = expectedDate;

		// Assert
		Assert.Equal(expectedDate, book.DateFinishedReading);
	}

	[Fact]
	public void Book_Review_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedReview = "Great book! Highly recommend.";

		// Act
		book.Review = expectedReview;

		// Assert
		Assert.Equal(expectedReview, book.Review);
	}

	[Fact]
	public void Book_Rating_CanBeSet()
	{
		// Arrange
		var book = new Book();
		var expectedRating = 5;

		// Act
		book.Rating = expectedRating;

		// Assert
		Assert.Equal(expectedRating, book.Rating);
	}

	[Fact]
	public void Book_AllPropertiesCanBeSetTogether()
	{
		// Arrange & Act
		var book = new Book
		{
			Id = 1,
			Title = "Test Book",
			Author = "Test Author",
			DateBought = new DateTime(2023, 1, 1),
			ReasonForBuying = "Test Reason",
			Status = BookStatus.Read,
			ImagePath = "/path/to/image.jpg",
			Isbn = "1234567890",
			ShortDescription = "Test Description",
			DateStartedReading = new DateTime(2023, 1, 5),
			DateFinishedReading = new DateTime(2023, 1, 20),
			Review = "Test Review",
			Rating = 4,
		};

		// Assert
		Assert.Equal(1, book.Id);
		Assert.Equal("Test Book", book.Title);
		Assert.Equal("Test Author", book.Author);
		Assert.Equal(new DateTime(2023, 1, 1), book.DateBought);
		Assert.Equal("Test Reason", book.ReasonForBuying);
		Assert.Equal(BookStatus.Read, book.Status);
		Assert.Equal("/path/to/image.jpg", book.ImagePath);
		Assert.Equal("1234567890", book.Isbn);
		Assert.Equal("Test Description", book.ShortDescription);
		Assert.Equal(new DateTime(2023, 1, 5), book.DateStartedReading);
		Assert.Equal(new DateTime(2023, 1, 20), book.DateFinishedReading);
		Assert.Equal("Test Review", book.Review);
		Assert.Equal(4, book.Rating);
	}
}
