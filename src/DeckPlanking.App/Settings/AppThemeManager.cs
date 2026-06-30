namespace DeckPlanking.App.Settings;

public static class AppThemeManager
{
    public static void Apply(AppThemeOption theme)
    {
        var application = Application.Current;
        if (application is null)
        {
            return;
        }

        var palette = GetPalette(theme);
        application.Resources["AppBackgroundColor"] = Color.FromArgb(palette.Background);
        application.Resources["AppSurfaceColor"] = Color.FromArgb(palette.Surface);
        application.Resources["AppPrimaryTextColor"] = Color.FromArgb(palette.PrimaryText);
        application.Resources["AppSecondaryTextColor"] = Color.FromArgb(palette.SecondaryText);
        application.Resources["AppAccentColor"] = Color.FromArgb(palette.Accent);
    }

    private static AppThemePalette GetPalette(AppThemeOption theme)
    {
        return theme switch
        {
            AppThemeOption.Dark => new("#121416", "#1E2428", "#F2F5F7", "#B7C0C8", "#7BA7BC"),
            AppThemeOption.Blue => new("#EEF6FA", "#FFFFFF", "#102A43", "#486581", "#1976A2"),
            AppThemeOption.Saffron => new("#FFF7E0", "#FFFFFF", "#2E2414", "#6E5A2E", "#C58A00"),
            AppThemeOption.DarkRed => new("#1C1114", "#2B1A1F", "#F7EEF0", "#D8B7BF", "#B6404A"),
            _ => new("#F7F9FB", "#FFFFFF", "#17202A", "#52616F", "#2F6F8F")
        };
    }

    private sealed record AppThemePalette(
        string Background,
        string Surface,
        string PrimaryText,
        string SecondaryText,
        string Accent);
}
