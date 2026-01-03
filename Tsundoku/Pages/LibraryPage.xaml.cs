using Tsundoku.ViewModels;

namespace Tsundoku.Pages;

public partial class LibraryPage : ContentPage
{
	private readonly LibraryViewModel _viewModel;

	public LibraryPage(LibraryViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.LoadCommand.Execute(null);
	}
}
