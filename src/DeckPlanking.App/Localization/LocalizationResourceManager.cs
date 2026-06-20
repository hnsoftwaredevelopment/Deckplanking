using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace DeckPlanking.App.Localization;

public sealed class LocalizationResourceManager : INotifyPropertyChanged
{
    private readonly ResourceManager resourceManager = new(
        "DeckPlanking.App.Resources.Strings.AppResources",
        typeof(LocalizationResourceManager).Assembly);

    private LocalizationResourceManager()
    {
    }

    public static LocalizationResourceManager Instance { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentUICulture;

    public string this[string key] => resourceManager.GetString(key, CurrentCulture) ?? key;

    public void SetCulture(CultureInfo culture)
    {
        CurrentCulture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }
}
