using DeckPlanking.Core.Measurement;

namespace DeckPlanking.App.Settings;

public static class DisplayLengthFormatter
{
    private const int MetricDecimalPlaces = 1;
    private const int ImperialFractionDenominator = 16;
    private const double MillimetersPerInch = 25.4d;

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

    public static double ToInputValue(double millimeters)
    {
        return AppPreferencesStore.GetDisplayUnitSystem() == DisplayUnitSystemOption.Imperial
            ? Math.Round(millimeters / MillimetersPerInch, 4, MidpointRounding.AwayFromZero)
            : millimeters;
    }

    public static double FromInputValue(double displayValue)
    {
        return AppPreferencesStore.GetDisplayUnitSystem() == DisplayUnitSystemOption.Imperial
            ? displayValue * MillimetersPerInch
            : displayValue;
    }

    public static string InputUnitText =>
        AppPreferencesStore.GetDisplayUnitSystem() == DisplayUnitSystemOption.Imperial
            ? "in"
            : "mm";
}
