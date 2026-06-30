namespace DeckPlanking.Core.Generation;

public sealed record PlankRow(
    int RowNumber,
    int Phase,
    int SeamOffsetSegments,
    IReadOnlyList<decimal> SeamPositionsMillimeters);
