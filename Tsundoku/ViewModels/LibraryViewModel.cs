using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tsundoku.Models;
using Tsundoku.Services;

namespace Tsundoku.ViewModels;

public partial class LibraryViewModel(IBookRepository repository) : ObservableObject
{
	public ObservableCollection<Book> Books { get; } = new();

	[ObservableProperty]
	private bool isBusy;

	[RelayCommand]
	public async Task LoadAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			var books = await repository.GetAllAsync().ConfigureAwait(false);

			MainThread.BeginInvokeOnMainThread(() =>
			{
				Books.Clear();
				foreach (var book in books)
					Books.Add(book);
			});
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	private async Task AddAsync()
	{
		await Shell.Current.GoToAsync(nameof(Pages.BookEditPage));
	}

	[RelayCommand]
	private async Task OpenAsync(Book? book)
	{
		if (book is null)
			return;

		await Shell.Current.GoToAsync($"{nameof(Pages.BookEditPage)}?id={book.Id}");
	}

	[RelayCommand]
	private async Task DeleteAsync(Book? book)
	{
		if (book is null)
			return;

		await repository.DeleteAsync(book.Id).ConfigureAwait(false);
		MainThread.BeginInvokeOnMainThread(() => Books.Remove(book));
	}
}
