namespace DeckPlanking.App.Settings;

public sealed class AppPreferencesChangedEventArgs(string preferenceName) : EventArgs
{
    public string PreferenceName { get; } = preferenceName;
}
