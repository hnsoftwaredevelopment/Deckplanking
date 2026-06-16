namespace DeckPlanking.Core.Measurement;

public static class ScaleLengthCalculator
{
    public static decimal CalculateScaleLengthMillimeters(
        decimal realLength,
        LengthUnit realLengthUnit,
        ScaleSettings scaleSettings)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(realLength);

        var actualMeters = realLengthUnit == LengthUnit.Meters
            ? realLength
            : realLength * 0.3048m;

        var scaleFactor = scaleSettings.Mode == ScaleMode.Decimal
            ? 1m / scaleSettings.DecimalRatio
            : scaleSettings.InchesPerFoot / 12m;

        return actualMeters * scaleFactor * 1000m;
    }
}
