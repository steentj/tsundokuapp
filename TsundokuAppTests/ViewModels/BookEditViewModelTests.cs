using Moq;
using Tsundoku.Models;
using Tsundoku.Services;
using Tsundoku.ViewModels;

namespace TsundokuAppTests.ViewModels;

public class BookEditViewModelTests
{
	[Fact]
	public void Constructor_InitializesDefaultValues()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();

		// Act
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Assert
		Assert.Equal(0, viewModel.Id);
		Assert.Equal(string.Empty, viewModel.Title);
		Assert.Equal(string.Empty, viewModel.Author);
		Assert.Equal(DateTime.Today, viewModel.DateBought);
		Assert.Equal(string.Empty, viewModel.ReasonForBuying);
		Assert.Equal(BookStatus.Bought, viewModel.Status);
		Assert.NotNull(viewModel.StatusOptions);
		Assert.Empty(viewModel.StatusOptions);
	}

	[Fact]
	public async Task InitializeAsync_WithNullBookId_InitializesForNewBook()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		await viewModel.InitializeAsync(null);

		// Assert
		Assert.Equal(0, viewModel.Id);
		Assert.Equal(string.Empty, viewModel.Title);
		Assert.Equal(string.Empty, viewModel.Author);
		Assert.Equal(BookStatus.Bought, viewModel.Status);
		Assert.Null(viewModel.ValidationMessage);
	}

	[Fact]
	public async Task InitializeAsync_WithZeroBookId_InitializesForNewBook()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		await viewModel.InitializeAsync(0);

		// Assert
		Assert.Equal(0, viewModel.Id);
		Assert.Equal(string.Empty, viewModel.Title);
	}

	[Fact]
	public async Task InitializeAsync_WithValidBookId_LoadsBook()
	{
		// Arrange
		var book = new Book
		{
			Id = 1,
			Title = "Test Book",
			Author = "Test Author",
			DateBought = new DateTime(2023, 1, 1),
			ReasonForBuying = "Test Reason",
			Status = BookStatus.Read,
			Rating = 4
		};

		var mockRepository = new Mock<IBookRepository>();
		mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(book);

		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		await viewModel.InitializeAsync(1);

		// Assert
		Assert.Equal(1, viewModel.Id);
		Assert.Equal("Test Book", viewModel.Title);
		Assert.Equal("Test Author", viewModel.Author);
		Assert.Equal(new DateTime(2023, 1, 1), viewModel.DateBought);
		Assert.Equal("Test Reason", viewModel.ReasonForBuying);
		Assert.Equal(BookStatus.Read, viewModel.Status);
		Assert.Equal(4, viewModel.RatingValue);
	}

	[Fact]
	public async Task InitializeAsync_WithNonExistentBookId_DoesNotCrash()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Book?)null);

		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		await viewModel.InitializeAsync(999);

		// Assert - Should not throw
		Assert.True(true);
	}

	[Fact]
	public async Task InitializeAsync_PopulatesStatusOptions()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		await viewModel.InitializeAsync(null);

		// Assert
		Assert.Equal(4, viewModel.StatusOptions.Count);
		Assert.Contains(viewModel.StatusOptions, o => o.Value == BookStatus.Bought);
		Assert.Contains(viewModel.StatusOptions, o => o.Value == BookStatus.StartedReading);
		Assert.Contains(viewModel.StatusOptions, o => o.Value == BookStatus.Read);
		Assert.Contains(viewModel.StatusOptions, o => o.Value == BookStatus.GivenUp);
	}

	[Fact]
	public async Task LookupIsbnAsync_WithEmptyIsbn_SetsValidationMessage()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);
		
		viewModel.Isbn = "";

		// Act
		await viewModel.LookupIsbnCommand.ExecuteAsync(null);

		// Assert
		Assert.NotNull(viewModel.ValidationMessage);
	}

	[Fact]
	public async Task LookupIsbnAsync_WithValidIsbn_UpdatesBookDetails()
	{
		// Arrange
		var lookupResult = new BookLookupResult
		{
			Title = "Lookup Title",
			Author = "Lookup Author",
			ShortDescription = "Lookup Description",
			CoverImageUrl = "https://example.com/cover.jpg"
		};

		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		mockBookLookup.Setup(l => l.LookupByIsbnAsync("1234567890", It.IsAny<CancellationToken>()))
			.ReturnsAsync(lookupResult);

		var mockImageService = new Mock<IImageService>();
		mockImageService.Setup(i => i.DownloadAndStoreAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync("/path/to/image.jpg");

		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		viewModel.Isbn = "1234567890";

		// Act
		await viewModel.LookupIsbnCommand.ExecuteAsync(null);

		// Assert
		Assert.Equal("Lookup Title", viewModel.Title);
		Assert.Equal("Lookup Author", viewModel.Author);
		Assert.Equal("Lookup Description", viewModel.ShortDescription);
		Assert.Equal("/path/to/image.jpg", viewModel.ImagePath);
	}

	[Fact]
	public async Task LookupIsbnAsync_DoesNotOverwriteExistingTitle()
	{
		// Arrange
		var lookupResult = new BookLookupResult
		{
			Title = "Lookup Title",
			Author = "Lookup Author"
		};

		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		mockBookLookup.Setup(l => l.LookupByIsbnAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(lookupResult);

		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		viewModel.Title = "Existing Title";
		viewModel.Isbn = "1234567890";

		// Act
		await viewModel.LookupIsbnCommand.ExecuteAsync(null);

		// Assert
		Assert.Equal("Existing Title", viewModel.Title);
		Assert.Equal("Lookup Author", viewModel.Author);
	}

	[Fact]
	public async Task PickPhotoAsync_WithSuccessfulPick_UpdatesImagePath()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		mockImageService.Setup(i => i.PickPhotoAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync("/path/to/picked/image.jpg");

		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		await viewModel.PickPhotoCommand.ExecuteAsync(null);

		// Assert
		Assert.Equal("/path/to/picked/image.jpg", viewModel.ImagePath);
	}

	[Fact]
	public async Task PickFileAsync_WithSuccessfulPick_UpdatesImagePath()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		mockImageService.Setup(i => i.PickImageFileAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync("/path/to/picked/file.jpg");

		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		await viewModel.PickFileCommand.ExecuteAsync(null);

		// Assert
		Assert.Equal("/path/to/picked/file.jpg", viewModel.ImagePath);
	}

	[Fact]
	public async Task TakePhotoAsync_WithSuccessfulCapture_UpdatesImagePath()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		mockImageService.Setup(i => i.CapturePhotoAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync("/path/to/captured/photo.jpg");

		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		await viewModel.TakePhotoCommand.ExecuteAsync(null);

		// Assert
		Assert.Equal("/path/to/captured/photo.jpg", viewModel.ImagePath);
	}

	[Fact]
	public void SelectedStatusOption_WhenChanged_UpdatesStatus()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		var statusOption = new StatusOption(BookStatus.Read, "Read");

		// Act
		viewModel.SelectedStatusOption = statusOption;

		// Assert
		Assert.Equal(BookStatus.Read, viewModel.Status);
	}

	[Fact]
	public void HasStartedReading_CanBeSet()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		viewModel.HasStartedReading = true;

		// Assert
		Assert.True(viewModel.HasStartedReading);
	}

	[Fact]
	public void HasFinishedReading_CanBeSet()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		viewModel.HasFinishedReading = true;

		// Assert
		Assert.True(viewModel.HasFinishedReading);
	}

	[Fact]
	public void RatingValue_CanBeSet()
	{
		// Arrange
		var mockRepository = new Mock<IBookRepository>();
		var mockBookLookup = new Mock<IBookLookupService>();
		var mockImageService = new Mock<IImageService>();
		var viewModel = new BookEditViewModel(
			mockRepository.Object,
			mockBookLookup.Object,
			mockImageService.Object);

		// Act
		viewModel.RatingValue = 4.5;

		// Assert
		Assert.Equal(4.5, viewModel.RatingValue);
	}
}
