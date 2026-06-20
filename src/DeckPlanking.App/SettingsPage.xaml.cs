using DeckPlanking.App.Settings;

namespace DeckPlanking.App;

public partial class SettingsPage : ContentPage
{
    private readonly List<AppPreferenceItem<AppLanguageOption>> languageOptions;
    private readonly List<AppPreferenceItem<AppThemeOption>> themeOptions;
    private readonly List<AppPreferenceItem<DisplayUnitSystemOption>> displayUnitSystemOptions;
    private bool isLoading;

    public SettingsPage()
    {
        InitializeComponent();

        languageOptions = AppPreferencesStore.LanguageOptions
            .Select(language => new AppPreferenceItem<AppLanguageOption>(language, language.DisplayName))
            .ToList();

        themeOptions =
        [
            new(AppThemeOption.Light, "Light"),
            new(AppThemeOption.Dark, "Dark"),
            new(AppThemeOption.Blue, "Blue"),
            new(AppThemeOption.Saffron, "Saffron"),
            new(AppThemeOption.DarkRed, "Dark red")
        ];

        displayUnitSystemOptions =
        [
            new(DisplayUnitSystemOption.Metric, "Metric"),
            new(DisplayUnitSystemOption.Imperial, "Imperial")
        ];

        LoadSettings();
    }

    private void LoadSettings()
    {
        isLoading = true;
        try
        {
            LanguagePicker.ItemsSource = languageOptions;
            ThemePicker.ItemsSource = themeOptions;
            DisplayUnitSystemPicker.ItemsSource = displayUnitSystemOptions;

            LanguagePicker.SelectedItem = languageOptions.First(option =>
                option.Value.CultureName == AppPreferencesStore.GetLanguageCultureName());
            ThemePicker.SelectedItem = themeOptions.First(option =>
                option.Value == AppPreferencesStore.GetTheme());
            DisplayUnitSystemPicker.SelectedItem = displayUnitSystemOptions.First(option =>
                option.Value == AppPreferencesStore.GetDisplayUnitSystem());
        }
        finally
        {
            isLoading = false;
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        if (isLoading || LanguagePicker.SelectedItem is not AppPreferenceItem<AppLanguageOption> selectedLanguage)
        {
            return;
        }

        AppPreferencesStore.SetLanguageCultureName(selectedLanguage.Value.CultureName);
        AppCultureManager.Apply(selectedLanguage.Value.CultureName);
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        if (isLoading || ThemePicker.SelectedItem is not AppPreferenceItem<AppThemeOption> selectedTheme)
        {
            return;
        }

        AppPreferencesStore.SetTheme(selectedTheme.Value);
        AppThemeManager.Apply(selectedTheme.Value);
    }

    private void OnDisplayUnitSystemChanged(object? sender, EventArgs e)
    {
        if (isLoading || DisplayUnitSystemPicker.SelectedItem is not AppPreferenceItem<DisplayUnitSystemOption> selectedDisplayUnitSystem)
        {
            return;
        }

        AppPreferencesStore.SetDisplayUnitSystem(selectedDisplayUnitSystem.Value);
    }
}
