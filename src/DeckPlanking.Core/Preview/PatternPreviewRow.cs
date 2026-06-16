namespace DeckPlanking.Core.Preview;

public sealed record PatternPreviewRow(
    int RowNumber,
    int SeamOffsetSegments,
    IReadOnlyList<decimal> SeamPositionsMillimeters,
    string SeamPositionsText);
