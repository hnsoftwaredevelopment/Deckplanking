using System.Reflection;
namespace DeckPlanking.App;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();

        VersionLabel.Text = GetApplicationVersion();
        PlatformLabel.Text = DeviceInfo.Current.Platform.ToString();
        OsVersionLabel.Text = DeviceInfo.Current.VersionString;
    }

    private async void OnFeedbackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(FeedbackPage));
    }

    private async void OnUserGuideClicked(object? sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new UserGuidePage());
    }

    internal static string GetApplicationVersion()
    {
        var assembly = typeof(App).Assembly;
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version
            ?? AppInfo.Current.VersionString;
    }
}
