using System.Globalization;
using System.Resources;
using Microsoft.Maui.Controls.Xaml;

namespace Tsundoku.MarkupExtensions;

[ContentProperty(nameof(Key))]
public sealed class TranslateExtension : IMarkupExtension<string>
{
	private static readonly ResourceManager ResourceManager = new(
		"Tsundoku.Resources.Strings.AppResources",
		typeof(TranslateExtension).Assembly);

	public string Key { get; set; } = string.Empty;

	public string ProvideValue(IServiceProvider serviceProvider)
	{
		if (string.IsNullOrWhiteSpace(Key))
			return string.Empty;

		return ResourceManager.GetString(Key, CultureInfo.CurrentUICulture) ?? Key;
	}

	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}
