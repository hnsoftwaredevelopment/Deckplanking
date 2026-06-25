namespace DeckPlanking.Core.Configuration;

public sealed record DeckContourSettings(
    DeckShapeKind Shape,
    decimal BowWidthPercentage,
    decimal SternWidthPercentage,
    decimal BowTaperLengthPercentage = 25m,
    decimal SternTaperLengthPercentage = 10m,
    decimal BowRoundnessPercentage = 0m,
    decimal SternRoundnessPercentage = 0m)
{
    public static DeckContourSettings Rectangular { get; } = new(
        DeckShapeKind.Rectangular,
        BowWidthPercentage: 100m,
        SternWidthPercentage: 100m,
        BowTaperLengthPercentage: 25m,
        SternTaperLengthPercentage: 10m,
        BowRoundnessPercentage: 0m,
        SternRoundnessPercentage: 0m);
}
