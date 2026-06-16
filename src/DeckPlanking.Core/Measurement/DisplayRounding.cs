namespace DeckPlanking.Core.Measurement;

public static class DisplayRounding
{
    public static decimal RoundMillimeters(decimal millimeters, int decimalPlaces)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(decimalPlaces);
        return decimal.Round(millimeters, decimalPlaces, MidpointRounding.AwayFromZero);
    }

    public static decimal RoundInchesToFraction(decimal inches, int denominator)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(denominator);
        var roundedNumerator = decimal.Round(inches * denominator, 0, MidpointRounding.AwayFromZero);
        return roundedNumerator / denominator;
    }
}
