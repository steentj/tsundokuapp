namespace Tsundoku;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(Pages.BookEditPage), typeof(Pages.BookEditPage));
	}
}
