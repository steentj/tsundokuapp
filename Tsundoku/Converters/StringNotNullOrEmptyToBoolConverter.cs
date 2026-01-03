using System.Globalization;
using Microsoft.Maui.Controls;

namespace Tsundoku.Converters;

public sealed class StringNotNullOrEmptyToBoolConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is string s && !string.IsNullOrWhiteSpace(s);
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> throw new NotSupportedException();
}
