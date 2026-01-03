using System.Globalization;
using System.Resources;
using Microsoft.Maui.Controls;
using Tsundoku.Models;

namespace Tsundoku.Converters;

public sealed class BookStatusToLocalizedStringConverter : IValueConverter
{
	private static readonly ResourceManager ResourceManager = new(
		"Tsundoku.Resources.Strings.AppResources",
		typeof(BookStatusToLocalizedStringConverter).Assembly);

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not BookStatus status)
			return string.Empty;

		var key = status switch
		{
			BookStatus.Bought => "StatusBought",
			BookStatus.StartedReading => "StatusStartedReading",
			BookStatus.Read => "StatusRead",
			BookStatus.GivenUp => "StatusGivenUp",
			_ => "StatusBought",
		};

		return ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> throw new NotSupportedException();
}
