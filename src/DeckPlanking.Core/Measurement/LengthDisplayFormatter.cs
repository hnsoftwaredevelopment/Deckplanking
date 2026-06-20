namespace DeckPlanking.Core.Measurement;

using System.Globalization;

public static class LengthDisplayFormatter
{
    private const decimal MillimetersPerInch = 25.4m;

    public static string FormatMillimeters(decimal millimeters, int decimalPlaces)
    {
        var rounded = DisplayRounding.RoundMillimeters(millimeters, decimalPlaces);
        return $"{rounded.ToString("0.#", CultureInfo.InvariantCulture)} mm";
    }

    public static string FormatInchesFromMillimeters(decimal millimeters, int denominator)
    {
        var roundedInches = DisplayRounding.RoundInchesToFraction(millimeters / MillimetersPerInch, denominator);
        var wholeInches = decimal.Truncate(roundedInches);
        var fractionalInches = roundedInches - wholeInches;
        var numerator = (int)decimal.Round(fractionalInches * denominator, 0, MidpointRounding.AwayFromZero);

        if (numerator == denominator)
        {
            wholeInches += 1;
            numerator = 0;
        }

        if (numerator == 0)
        {
            return $"{wholeInches.ToString("0", CultureInfo.InvariantCulture)} in";
        }

        var divisor = GreatestCommonDivisor(numerator, denominator);
        numerator /= divisor;
        var simplifiedDenominator = denominator / divisor;

        if (wholeInches == 0)
        {
            return $"{numerator}/{simplifiedDenominator} in";
        }

        return $"{wholeInches.ToString("0", CultureInfo.InvariantCulture)} {numerator}/{simplifiedDenominator} in";
    }

    private static int GreatestCommonDivisor(int left, int right)
    {
        while (right != 0)
        {
            var remainder = left % right;
            left = right;
            right = remainder;
        }

        return Math.Abs(left);
    }
}
