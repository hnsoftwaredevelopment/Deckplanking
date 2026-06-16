namespace DeckPlanking.App.ViewModels;

public sealed record OptionItem<TValue>(string DisplayName, TValue Value);
