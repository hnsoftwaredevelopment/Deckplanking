namespace DeckPlanking.App.Settings;

public sealed record AppPreferenceItem<TValue>(TValue Value, string DisplayName);
