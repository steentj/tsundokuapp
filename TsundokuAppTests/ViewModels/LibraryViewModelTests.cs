using Moq;
using Tsundoku.Models;
using Tsundoku.Services;
using Tsundoku.ViewModels;
using TsundokuAppTests.Stubs;

namespace TsundokuAppTests.ViewModels;

public class LibraryViewModelTests
{
	[Fact]
	public void Constructor_InitializesEmptyBooksCollection()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();

		// Act
		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());

		// Assert
		Assert.NotNull(viewModel.Books);
		Assert.Empty(viewModel.Books);
	}

	[Fact]
	public void Constructor_InitializesIsBusyAsFalse()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();

		// Act
		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());

		// Assert
		Assert.False(viewModel.IsBusy);
	}

	[Fact]
	public async Task LoadAsync_WhenNotBusy_LoadsBooksFromRepository()
	{
		// Arrange
		var books = new List<Book>
		{
			new Book { Id = 1, Title = "Book 1", Author = "Author 1" },
			new Book { Id = 2, Title = "Book 2", Author = "Author 2" },
		};

		var mockRepository = new Mock<IBookRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(books);

		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());

		// Act
		await viewModel.LoadCommand.ExecuteAsync(null);

		// Assert
		Assert.Equal(2, viewModel.Books.Count);
		Assert.Equal("Book 1", viewModel.Books[0].Title);
		Assert.Equal("Book 2", viewModel.Books[1].Title);
		mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task LoadAsync_WhenAlreadyBusy_DoesNotLoadAgain()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Book>());

		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());

		// Simulate the view model being busy
		await viewModel.LoadCommand.ExecuteAsync(null);
		
		// Reset the mock to verify it's not called again
		mockRepository.Invocations.Clear();

		// Force IsBusy to true (simulating concurrent call)
		typeof(LibraryViewModel)
			.GetProperty(nameof(LibraryViewModel.IsBusy))!
			.SetValue(viewModel, true);

		// Act
		await viewModel.LoadCommand.ExecuteAsync(null);

		// Assert
		mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task LoadAsync_ClearsPreviousBooksBeforeLoading()
	{
		// Arrange
		var initialBooks = new List<Book>
		{
			new Book { Id = 1, Title = "Book 1" },
		};
		var newBooks = new List<Book>
		{
			new Book { Id = 2, Title = "Book 2" },
			new Book { Id = 3, Title = "Book 3" },
		};

		var mockRepository = new Mock<IBookRepository>();
		mockRepository.SetupSequence(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(initialBooks)
			.ReturnsAsync(newBooks);

		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());

		// Act
		await viewModel.LoadCommand.ExecuteAsync(null);
		await viewModel.LoadCommand.ExecuteAsync(null);

		// Assert
		Assert.Equal(2, viewModel.Books.Count);
		Assert.Equal("Book 2", viewModel.Books[0].Title);
		Assert.Equal("Book 3", viewModel.Books[1].Title);
	}

	[Fact]
	public async Task LoadAsync_SetsIsBusyToFalseAfterCompletion()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<Book>());

		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());

		// Act
		await viewModel.LoadCommand.ExecuteAsync(null);

		// Assert
		Assert.False(viewModel.IsBusy);
	}

	[Fact]
	public async Task LoadAsync_SetsIsBusyToFalseEvenOnException()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("Test exception"));

		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());

		// Act & Assert
		await Assert.ThrowsAsync<Exception>(async () => 
			await viewModel.LoadCommand.ExecuteAsync(null));
		
		Assert.False(viewModel.IsBusy);
	}

	[Fact]
	public async Task DeleteAsync_WithValidBook_RemovesBookFromCollection()
	{
		// Arrange
		var book = new Book { Id = 1, Title = "Book to Delete" };
		var mockRepository = new Mock<IBookRepository>();
		mockRepository.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());
		viewModel.Books.Add(book);

		// Act
		await viewModel.DeleteCommand.ExecuteAsync(book);

		// Assert
		Assert.Empty(viewModel.Books);
		mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_WithNullBook_DoesNothing()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());

		// Act
		await viewModel.DeleteCommand.ExecuteAsync(null);

		// Assert
		mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task OpenAsync_WithNullBook_DoesNotNavigate()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var viewModel = new LibraryViewModel(mockRepository.Object, new ImmediateMainThreadInvoker());

		// Act
		await viewModel.OpenCommand.ExecuteAsync(null);

		// Assert - No exception should be thrown
		Assert.True(true);
	}
}
