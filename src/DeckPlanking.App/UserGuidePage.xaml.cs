using DeckPlanking.App.Documentation;
using DeckPlanking.App.Localization;
using DeckPlanking.Core.Documentation;

namespace DeckPlanking.App;

public partial class UserGuidePage : ContentPage
{
    private bool hasLoaded;

    public UserGuidePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (hasLoaded)
        {
            return;
        }

        hasLoaded = true;
        await LoadGuideAsync();
    }

    private async Task LoadGuideAsync()
    {
        try
        {
            var markdown = await UserGuideAssetReader.ReadForCurrentCultureAsync();
            var html = UserGuideMarkdownRenderer.RenderHtml(markdown, CreateTheme());
            GuideWebView.Source = new HtmlWebViewSource { Html = html };
        }
        catch (Exception exception)
        {
            var resources = LocalizationResourceManager.Instance;
            await DisplayAlertAsync(resources["UserGuideLoadFailed"], exception.Message, resources["Ok"]);
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private static UserGuideHtmlTheme CreateTheme()
    {
        return new UserGuideHtmlTheme(
            GetColor("AppBackgroundColor", "#f7f8fa"),
            GetColor("AppSurfaceColor", "#ffffff"),
            GetColor("AppPrimaryTextColor", "#111827"),
            GetColor("AppSecondaryTextColor", "#4b5563"),
            GetColor("AppAccentColor", "#4f2bd8"));
    }

    private static string GetColor(string resourceKey, string fallback)
    {
        if (Application.Current?.Resources.TryGetValue(resourceKey, out var value) == true && value is Color color)
        {
            return color.ToHex();
        }

        return fallback;
    }
}
