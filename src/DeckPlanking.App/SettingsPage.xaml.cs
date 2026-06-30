using DeckPlanking.App.Settings;
using DeckPlanking.App.Localization;

namespace DeckPlanking.App;

public partial class SettingsPage : ContentPage
{
    private List<AppPreferenceItem<AppLanguageOption>> languageOptions = [];
    private List<AppPreferenceItem<AppThemeOption>> themeOptions = [];
    private List<AppPreferenceItem<DisplayUnitSystemOption>> displayUnitSystemOptions = [];
    private bool isLoading;

    public SettingsPage()
    {
        InitializeComponent();

        BuildOptions();
        LoadSettings();
    }

    private void BuildOptions()
    {
        languageOptions = AppPreferencesStore.LanguageOptions
            .Select(language => new AppPreferenceItem<AppLanguageOption>(language, language.DisplayName))
            .ToList();

        themeOptions =
        [
            new(AppThemeOption.Light, T("ThemeLight")),
            new(AppThemeOption.Dark, T("ThemeDark")),
            new(AppThemeOption.Blue, T("ThemeBlue")),
            new(AppThemeOption.Saffron, T("ThemeSaffron")),
            new(AppThemeOption.DarkRed, T("ThemeDarkRed"))
        ];

        displayUnitSystemOptions =
        [
            new(DisplayUnitSystemOption.Metric, T("Metric")),
            new(DisplayUnitSystemOption.Imperial, T("Imperial"))
        ];
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
        BuildOptions();
        LoadSettings();
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

    private static string T(string key)
    {
        return LocalizationResourceManager.Instance[key];
    }
}
