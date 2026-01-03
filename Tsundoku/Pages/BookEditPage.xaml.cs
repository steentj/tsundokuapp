namespace Tsundoku.Pages;

[QueryProperty(nameof(BookId), "id")]
public partial class BookEditPage : ContentPage
{
	private readonly ViewModels.BookEditViewModel _viewModel;

	public BookEditPage(ViewModels.BookEditViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	public string? BookId { get; set; }

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		int? id = null;
		if (int.TryParse(BookId, out var parsed))
			id = parsed;

		await _viewModel.InitializeAsync(id);
	}
}
