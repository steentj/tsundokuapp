using System.Globalization;

namespace Tsundoku.Localization;

public static class LocalizationConfigurator
{
	public static void ConfigureDefaultCulture()
	{
		var uiCulture = CultureInfo.CurrentUICulture;
		var useDanish = string.Equals(uiCulture.TwoLetterISOLanguageName, "da", StringComparison.OrdinalIgnoreCase);

		var culture = useDanish ? new CultureInfo("da-DK") : new CultureInfo("en-US");
		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = culture;
	}
}
