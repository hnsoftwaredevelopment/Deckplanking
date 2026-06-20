namespace DeckPlanking.App.Settings;

public static class AppPreferencesStore
{
    public const string DisplayUnitSystemPreferenceName = nameof(DisplayUnitSystemPreferenceName);

    private const string LanguageKey = "app.language";
    private const string ThemeKey = "app.theme";
    private const string DisplayUnitSystemKey = "app.displayUnitSystem";

    public const string DefaultLanguageCultureName = "en";
    public const AppThemeOption DefaultTheme = AppThemeOption.Light;
    public const DisplayUnitSystemOption DefaultDisplayUnitSystem = DisplayUnitSystemOption.Metric;

    public static IReadOnlyList<AppLanguageOption> LanguageOptions { get; } =
    [
        new("nl", "Nederlands"),
        new("en", "English"),
        new("de", "Deutsch"),
        new("fr", "Francais"),
        new("es", "Espanol"),
        new("it", "Italiano")
    ];

    public static event EventHandler<AppPreferencesChangedEventArgs>? PreferenceChanged;

    public static AppThemeOption GetTheme()
    {
        return ReadEnum(ThemeKey, DefaultTheme);
    }

    public static void SetTheme(AppThemeOption theme)
    {
        Preferences.Default.Set(ThemeKey, theme.ToString());
    }

    public static string GetLanguageCultureName()
    {
        var cultureName = Preferences.Default.Get(LanguageKey, DefaultLanguageCultureName);
        return LanguageOptions.Any(language => language.CultureName == cultureName)
            ? cultureName
            : DefaultLanguageCultureName;
    }

    public static void SetLanguageCultureName(string cultureName)
    {
        var normalizedCultureName = LanguageOptions.Any(language => language.CultureName == cultureName)
            ? cultureName
            : DefaultLanguageCultureName;

        Preferences.Default.Set(LanguageKey, normalizedCultureName);
    }

    public static DisplayUnitSystemOption GetDisplayUnitSystem()
    {
        return ReadEnum(DisplayUnitSystemKey, DefaultDisplayUnitSystem);
    }

    public static void SetDisplayUnitSystem(DisplayUnitSystemOption displayUnitSystem)
    {
        Preferences.Default.Set(DisplayUnitSystemKey, displayUnitSystem.ToString());
        PreferenceChanged?.Invoke(null, new AppPreferencesChangedEventArgs(DisplayUnitSystemPreferenceName));
    }

    private static TEnum ReadEnum<TEnum>(string key, TEnum defaultValue)
        where TEnum : struct, Enum
    {
        var value = Preferences.Default.Get(key, defaultValue.ToString());
        return Enum.TryParse<TEnum>(value, out var parsedValue)
            ? parsedValue
            : defaultValue;
    }
}
