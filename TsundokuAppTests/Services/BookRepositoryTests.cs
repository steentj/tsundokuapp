using Tsundoku.Data;
using Tsundoku.Models;
using Tsundoku.Services;

namespace TsundokuAppTests.Services;

public class BookRepositoryTests : IAsyncLifetime
{
	private TsundokuDb? _db;
	private BookRepository? _repository;

	public async Task InitializeAsync()
	{
		// Create a new database for each test
		_db = new TsundokuDb();
		await _db.InitializeAsync();
		_repository = new BookRepository(_db);
	}

	public async Task DisposeAsync()
	{
		// Clean up
		if (_db?.Connection != null)
		{
			await _db.Connection.CloseAsync();
		}
	}

	[Fact]
	public async Task GetAllAsync_WhenDatabaseIsEmpty_ReturnsEmptyList()
	{
		// Act
		var books = await _repository!.GetAllAsync();

		// Assert
		Assert.NotNull(books);
		Assert.Empty(books);
	}

	[Fact]
	public async Task SaveAsync_WithNewBook_InsertsBook()
	{
		// Arrange
		var book = new Book
		{
			Title = "Test Book",
			Author = "Test Author",
			DateBought = new DateTime(2023, 1, 1),
			ReasonForBuying = "Test Reason"
		};

		// Act
		var result = await _repository!.SaveAsync(book);
		var savedBooks = await _repository.GetAllAsync();

		// Assert
		Assert.True(result > 0);
		Assert.Single(savedBooks);
		Assert.Equal("Test Book", savedBooks[0].Title);
		Assert.Equal("Test Author", savedBooks[0].Author);
	}

	[Fact]
	public async Task SaveAsync_WithNewBook_SetsId()
	{
		// Arrange
		var book = new Book
		{
			Title = "Test Book",
			Author = "Test Author",
			ReasonForBuying = "Test Reason"
		};

		// Act
		await _repository!.SaveAsync(book);

		// Assert
		Assert.True(book.Id > 0);
	}

	[Fact]
	public async Task SaveAsync_WithNewBook_SetsUpdatedAtUtc()
	{
		// Arrange
		var beforeSave = DateTime.UtcNow.AddSeconds(-1);
		var book = new Book
		{
			Title = "Test Book",
			Author = "Test Author",
			ReasonForBuying = "Test Reason"
		};

		// Act
		await _repository!.SaveAsync(book);
		var afterSave = DateTime.UtcNow.AddSeconds(1);

		// Assert
		Assert.True(book.UpdatedAtUtc >= beforeSave);
		Assert.True(book.UpdatedAtUtc <= afterSave);
	}

	[Fact]
	public async Task SaveAsync_WithExistingBook_UpdatesBook()
	{
		// Arrange
		var book = new Book
		{
			Title = "Original Title",
			Author = "Original Author",
			ReasonForBuying = "Test Reason"
		};
		await _repository!.SaveAsync(book);

		// Act
		book.Title = "Updated Title";
		book.Author = "Updated Author";
		await _repository.SaveAsync(book);

		var updatedBook = await _repository.GetByIdAsync(book.Id);

		// Assert
		Assert.NotNull(updatedBook);
		Assert.Equal("Updated Title", updatedBook.Title);
		Assert.Equal("Updated Author", updatedBook.Author);
	}

	[Fact]
	public async Task SaveAsync_WithExistingBook_UpdatesUpdatedAtUtc()
	{
		// Arrange
		var book = new Book
		{
			Title = "Test Book",
			Author = "Test Author",
			ReasonForBuying = "Test Reason"
		};
		await _repository!.SaveAsync(book);
		var originalUpdatedAt = book.UpdatedAtUtc;

		// Wait a tiny bit to ensure timestamp is different
		await Task.Delay(10);

		// Act
		book.Title = "Updated Title";
		await _repository.SaveAsync(book);

		// Assert
		Assert.True(book.UpdatedAtUtc > originalUpdatedAt);
	}

	[Fact]
	public async Task GetByIdAsync_WithExistingId_ReturnsBook()
	{
		// Arrange
		var book = new Book
		{
			Title = "Test Book",
			Author = "Test Author",
			ReasonForBuying = "Test Reason"
		};
		await _repository!.SaveAsync(book);

		// Act
		var retrievedBook = await _repository.GetByIdAsync(book.Id);

		// Assert
		Assert.NotNull(retrievedBook);
		Assert.Equal(book.Id, retrievedBook.Id);
		Assert.Equal("Test Book", retrievedBook.Title);
		Assert.Equal("Test Author", retrievedBook.Author);
	}

	[Fact]
	public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
	{
		// Act
		var book = await _repository!.GetByIdAsync(999);

		// Assert
		Assert.Null(book);
	}

	[Fact]
	public async Task DeleteAsync_WithExistingId_RemovesBook()
	{
		// Arrange
		var book = new Book
		{
			Title = "Test Book",
			Author = "Test Author",
			ReasonForBuying = "Test Reason"
		};
		await _repository!.SaveAsync(book);

		// Act
		await _repository.DeleteAsync(book.Id);
		var deletedBook = await _repository.GetByIdAsync(book.Id);

		// Assert
		Assert.Null(deletedBook);
	}

	[Fact]
	public async Task GetAllAsync_ReturnsBooksSortedByDateBoughtDescending()
	{
		// Arrange
		var book1 = new Book
		{
			Title = "Book 1",
			Author = "Author 1",
			DateBought = new DateTime(2023, 1, 1),
			ReasonForBuying = "Reason 1"
		};
		var book2 = new Book
		{
			Title = "Book 2",
			Author = "Author 2",
			DateBought = new DateTime(2023, 3, 1),
			ReasonForBuying = "Reason 2"
		};
		var book3 = new Book
		{
			Title = "Book 3",
			Author = "Author 3",
			DateBought = new DateTime(2023, 2, 1),
			ReasonForBuying = "Reason 3"
		};

		await _repository!.SaveAsync(book1);
		await _repository.SaveAsync(book2);
		await _repository.SaveAsync(book3);

		// Act
		var books = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(3, books.Count);
		Assert.Equal("Book 2", books[0].Title); // Most recent
		Assert.Equal("Book 3", books[1].Title);
		Assert.Equal("Book 1", books[2].Title); // Oldest
	}

	[Fact]
	public async Task GetAllAsync_WithSameDateBought_SortsByTitleAscending()
	{
		// Arrange
		var sameDate = new DateTime(2023, 1, 1);
		var book1 = new Book
		{
			Title = "Zebra Book",
			Author = "Author",
			DateBought = sameDate,
			ReasonForBuying = "Reason"
		};
		var book2 = new Book
		{
			Title = "Apple Book",
			Author = "Author",
			DateBought = sameDate,
			ReasonForBuying = "Reason"
		};
		var book3 = new Book
		{
			Title = "Mango Book",
			Author = "Author",
			DateBought = sameDate,
			ReasonForBuying = "Reason"
		};

		await _repository!.SaveAsync(book1);
		await _repository.SaveAsync(book2);
		await _repository.SaveAsync(book3);

		// Act
		var books = await _repository.GetAllAsync();

		// Assert
		Assert.Equal(3, books.Count);
		Assert.Equal("Apple Book", books[0].Title);
		Assert.Equal("Mango Book", books[1].Title);
		Assert.Equal("Zebra Book", books[2].Title);
	}

	[Fact]
	public async Task SaveAsync_WithAllOptionalFields_SavesAllData()
	{
		// Arrange
		var book = new Book
		{
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
			Review = "Great book!",
			Rating = 5
		};

		// Act
		await _repository!.SaveAsync(book);
		var savedBook = await _repository.GetByIdAsync(book.Id);

		// Assert
		Assert.NotNull(savedBook);
		Assert.Equal("Test Book", savedBook.Title);
		Assert.Equal("Test Author", savedBook.Author);
		Assert.Equal(new DateTime(2023, 1, 1), savedBook.DateBought);
		Assert.Equal("Test Reason", savedBook.ReasonForBuying);
		Assert.Equal(BookStatus.Read, savedBook.Status);
		Assert.Equal("/path/to/image.jpg", savedBook.ImagePath);
		Assert.Equal("1234567890", savedBook.Isbn);
		Assert.Equal("Test Description", savedBook.ShortDescription);
		Assert.Equal(new DateTime(2023, 1, 5), savedBook.DateStartedReading);
		Assert.Equal(new DateTime(2023, 1, 20), savedBook.DateFinishedReading);
		Assert.Equal("Great book!", savedBook.Review);
		Assert.Equal(5, savedBook.Rating);
	}
}
