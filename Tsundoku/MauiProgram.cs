using Microsoft.Extensions.Logging;
using Tsundoku.Data;
using Tsundoku.Localization;
using Tsundoku.Pages;
using Tsundoku.Services;
using Tsundoku.ViewModels;

namespace Tsundoku;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		LocalizationConfigurator.ConfigureDefaultCulture();
		SQLitePCL.Batteries_V2.Init();

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<TsundokuDb>();
		builder.Services.AddSingleton<IBookRepository, BookRepository>();

		builder.Services.AddHttpClient<OpenLibraryBookLookupService>();
		builder.Services.AddSingleton<IBookLookupService>(sp => sp.GetRequiredService<OpenLibraryBookLookupService>());

		builder.Services.AddHttpClient<ImageService>();
		builder.Services.AddSingleton<IImageService>(sp => sp.GetRequiredService<ImageService>());

		builder.Services.AddTransient<LibraryViewModel>();
		builder.Services.AddTransient<BookEditViewModel>();

		builder.Services.AddTransient<LibraryPage>();
		builder.Services.AddTransient<BookEditPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
