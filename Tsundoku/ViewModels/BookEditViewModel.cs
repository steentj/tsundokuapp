using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tsundoku.Models;
using Tsundoku.Services;

namespace Tsundoku.ViewModels;

public sealed record StatusOption(BookStatus Value, string Display)
{
	public override string ToString() => Display;
}

public partial class BookEditViewModel(IBookRepository repository, IBookLookupService bookLookup, IImageService imageService) : ObservableObject
{
	private static readonly ResourceManager ResourceManager = new(
		"Tsundoku.Resources.Strings.AppResources",
		typeof(BookEditViewModel).Assembly);

	private Book? _loaded;

	public ObservableCollection<StatusOption> StatusOptions { get; } = new();

	[ObservableProperty]
	private StatusOption? selectedStatusOption;

	[ObservableProperty]
	private int id;

	[ObservableProperty]
	private string title = string.Empty;

	[ObservableProperty]
	private string author = string.Empty;

	[ObservableProperty]
	private DateTime dateBought = DateTime.Today;

	[ObservableProperty]
	private string reasonForBuying = string.Empty;

	[ObservableProperty]
	private string? isbn;

	[ObservableProperty]
	private string? shortDescription;

	[ObservableProperty]
	private bool hasStartedReading;

	[ObservableProperty]
	private DateTime startedReadingDate = DateTime.Today;

	[ObservableProperty]
	private bool hasFinishedReading;

	[ObservableProperty]
	private DateTime finishedReadingDate = DateTime.Today;

	[ObservableProperty]
	private BookStatus status = BookStatus.Bought;

	[ObservableProperty]
	private string? review;

	[ObservableProperty]
	private double ratingValue;

	[ObservableProperty]
	private string? imagePath;

	[ObservableProperty]
	private string? validationMessage;

	public async Task InitializeAsync(int? bookId)
	{
		BuildStatusOptions();

		if (bookId is null or 0)
		{
			_loaded = null;
			Id = 0;
			Title = string.Empty;
			Author = string.Empty;
			DateBought = DateTime.Today;
			ReasonForBuying = string.Empty;
			Isbn = null;
			ShortDescription = null;
			HasStartedReading = false;
			StartedReadingDate = DateTime.Today;
			HasFinishedReading = false;
			FinishedReadingDate = DateTime.Today;
			Status = BookStatus.Bought;
			SelectedStatusOption = StatusOptions.FirstOrDefault(o => o.Value == Status);
			Review = null;
			RatingValue = 0;
			ImagePath = null;
			ValidationMessage = null;
			return;
		}

		var book = await repository.GetByIdAsync(bookId.Value).ConfigureAwait(false);
		if (book is null)
			return;

		_loaded = book;
		Id = book.Id;
		Title = book.Title;
		Author = book.Author;
		DateBought = book.DateBought;
		ReasonForBuying = book.ReasonForBuying;
		Isbn = book.Isbn;
		ShortDescription = book.ShortDescription;
		HasStartedReading = book.DateStartedReading is not null;
		StartedReadingDate = book.DateStartedReading ?? DateTime.Today;
		HasFinishedReading = book.DateFinishedReading is not null;
		FinishedReadingDate = book.DateFinishedReading ?? DateTime.Today;
		Status = book.Status;
		SelectedStatusOption = StatusOptions.FirstOrDefault(o => o.Value == Status);
		Review = book.Review;
		RatingValue = book.Rating ?? 0;
		ImagePath = book.ImagePath;
		ValidationMessage = null;
	}

	partial void OnSelectedStatusOptionChanged(StatusOption? value)
	{
		if (value is not null)
			Status = value.Value;
	}

	private void BuildStatusOptions()
	{
		if (StatusOptions.Count > 0)
			return;

		StatusOptions.Add(new StatusOption(BookStatus.Bought, GetString("StatusBought")));
		StatusOptions.Add(new StatusOption(BookStatus.StartedReading, GetString("StatusStartedReading")));
		StatusOptions.Add(new StatusOption(BookStatus.Read, GetString("StatusRead")));
		StatusOptions.Add(new StatusOption(BookStatus.GivenUp, GetString("StatusGivenUp")));
	}

	private static string GetString(string key)
		=> ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;

	private bool Validate()
	{
		ValidationMessage = null;

		if (string.IsNullOrWhiteSpace(Title)
			|| string.IsNullOrWhiteSpace(Author)
			|| string.IsNullOrWhiteSpace(ReasonForBuying))
		{
			ValidationMessage = GetString("ValidationRequired");
			return false;
		}

		if (RatingValue is < 0 or > 5)
		{
			ValidationMessage = GetString("ValidationRating");
			return false;
		}

		if (HasFinishedReading && !HasStartedReading)
		{
			ValidationMessage = GetString("ValidationRequired");
			return false;
		}

		if (HasStartedReading && HasFinishedReading && FinishedReadingDate < StartedReadingDate)
		{
			ValidationMessage = GetString("ValidationRequired");
			return false;
		}

		return true;
	}

	[RelayCommand]
	private async Task SaveAsync()
	{
		if (!Validate())
			return;

		var book = _loaded ?? new Book();
		book.Title = Title.Trim();
		book.Author = Author.Trim();
		book.DateBought = DateBought;
		book.ReasonForBuying = ReasonForBuying.Trim();
		book.Status = Status;
		book.Isbn = string.IsNullOrWhiteSpace(Isbn) ? null : Isbn.Trim();
		book.ShortDescription = string.IsNullOrWhiteSpace(ShortDescription) ? null : ShortDescription.Trim();
		book.DateStartedReading = HasStartedReading ? StartedReadingDate : null;
		book.DateFinishedReading = HasFinishedReading ? FinishedReadingDate : null;
		book.Review = string.IsNullOrWhiteSpace(Review) ? null : Review.Trim();
		book.Rating = RatingValue == 0 ? null : (int)RatingValue;
		book.ImagePath = ImagePath;

		await repository.SaveAsync(book).ConfigureAwait(false);
		await Shell.Current.GoToAsync("..");
	}

	[RelayCommand]
	private async Task LookupIsbnAsync()
	{
		ValidationMessage = null;
		if (string.IsNullOrWhiteSpace(Isbn))
		{
			ValidationMessage = GetString("ValidationRequired");
			return;
		}

		var result = await bookLookup.LookupByIsbnAsync(Isbn).ConfigureAwait(false);
		if (result is null)
			return;

		if (string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(result.Title))
			Title = result.Title;
		if (string.IsNullOrWhiteSpace(Author) && !string.IsNullOrWhiteSpace(result.Author))
			Author = result.Author;
		if (string.IsNullOrWhiteSpace(ShortDescription) && !string.IsNullOrWhiteSpace(result.ShortDescription))
			ShortDescription = result.ShortDescription;

		if (string.IsNullOrWhiteSpace(ImagePath) && !string.IsNullOrWhiteSpace(result.CoverImageUrl))
		{
			var stored = await imageService.DownloadAndStoreAsync(result.CoverImageUrl).ConfigureAwait(false);
			if (!string.IsNullOrWhiteSpace(stored))
				ImagePath = stored;
		}
	}

	[RelayCommand]
	private async Task PickPhotoAsync()
	{
		var stored = await imageService.PickPhotoAsync().ConfigureAwait(false);
		if (!string.IsNullOrWhiteSpace(stored))
			ImagePath = stored;
	}

	[RelayCommand]
	private async Task PickFileAsync()
	{
		var stored = await imageService.PickImageFileAsync().ConfigureAwait(false);
		if (!string.IsNullOrWhiteSpace(stored))
			ImagePath = stored;
	}

	[RelayCommand]
	private async Task TakePhotoAsync()
	{
		var stored = await imageService.CapturePhotoAsync().ConfigureAwait(false);
		if (!string.IsNullOrWhiteSpace(stored))
			ImagePath = stored;
	}
}
