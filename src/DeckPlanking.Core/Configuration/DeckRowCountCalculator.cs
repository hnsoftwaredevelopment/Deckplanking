namespace DeckPlanking.Core.Configuration;

public static class DeckRowCountCalculator
{
    public static int CalculateRowsPerSide(
        decimal deckWidthMillimeters,
        decimal plankWidthMillimeters,
        bool useKingPlank)
    {
        if (deckWidthMillimeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(deckWidthMillimeters));
        }

        if (plankWidthMillimeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(plankWidthMillimeters));
        }

        var plankedWidth = useKingPlank
            ? Math.Max(0, deckWidthMillimeters - plankWidthMillimeters)
            : deckWidthMillimeters;

        var rowsPerSide = (int)Math.Ceiling(plankedWidth / (2 * plankWidthMillimeters));
        return Math.Max(1, rowsPerSide);
    }
}
