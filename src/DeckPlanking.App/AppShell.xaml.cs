namespace DeckPlanking.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
		Routing.RegisterRoute(nameof(FeedbackPage), typeof(FeedbackPage));
		Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
	}
}
