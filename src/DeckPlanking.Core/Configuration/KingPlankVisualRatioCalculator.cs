namespace DeckPlanking.Core.Configuration;

public static class KingPlankVisualRatioCalculator
{
    public static decimal Calculate(
        decimal plankWidthMillimeters,
        decimal kingPlankWidthMillimeters)
    {
        if (plankWidthMillimeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(plankWidthMillimeters));
        }

        if (kingPlankWidthMillimeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kingPlankWidthMillimeters));
        }

        return kingPlankWidthMillimeters / plankWidthMillimeters;
    }
}
