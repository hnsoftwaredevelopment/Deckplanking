using System.Globalization;
using DeckPlanking.App.Localization;

namespace DeckPlanking.App.Settings;

public static class AppCultureManager
{
    public static void Apply(string cultureName)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        LocalizationResourceManager.Instance.SetCulture(culture);
    }
}
