using DeckPlanking.Core.Measurement;

namespace DeckPlanking.App.Settings;

public static class DisplayLengthFormatter
{
    private const int MetricDecimalPlaces = 1;
    private const int ImperialFractionDenominator = 16;

    public static string Format(decimal millimeters)
    {
        return AppPreferencesStore.GetDisplayUnitSystem() == DisplayUnitSystemOption.Imperial
            ? LengthDisplayFormatter.FormatInchesFromMillimeters(millimeters, ImperialFractionDenominator)
            : LengthDisplayFormatter.FormatMillimeters(millimeters, MetricDecimalPlaces);
    }

    public static string FormatRulerLabel(decimal millimeters)
    {
        return AppPreferencesStore.GetDisplayUnitSystem() == DisplayUnitSystemOption.Imperial
            ? LengthDisplayFormatter.FormatInchesFromMillimeters(millimeters, ImperialFractionDenominator).Replace(" in", string.Empty, StringComparison.Ordinal)
            : $"{DisplayRounding.RoundMillimeters(millimeters, MetricDecimalPlaces):0.#}";
    }
}
