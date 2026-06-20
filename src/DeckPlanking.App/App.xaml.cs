using DeckPlanking.App.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace DeckPlanking.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        AppCultureManager.Apply(AppPreferencesStore.GetLanguageCultureName());
        AppThemeManager.Apply(AppPreferencesStore.GetTheme());
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
