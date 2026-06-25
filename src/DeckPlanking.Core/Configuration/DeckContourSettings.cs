namespace DeckPlanking.Core.Configuration;

public sealed record DeckContourSettings(
    DeckShapeKind Shape,
    decimal BowWidthPercentage,
    decimal SternWidthPercentage,
    decimal BowTaperLengthPercentage = 25m,
    decimal SternTaperLengthPercentage = 10m)
{
    public static DeckContourSettings Rectangular { get; } = new(
        DeckShapeKind.Rectangular,
        BowWidthPercentage: 100m,
        SternWidthPercentage: 100m,
        BowTaperLengthPercentage: 25m,
        SternTaperLengthPercentage: 10m);
}
