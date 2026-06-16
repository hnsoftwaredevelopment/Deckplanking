namespace DeckPlanking.Core.Preview;

public sealed record PatternPreviewRow(
    int RowNumber,
    int Phase,
    int SeamOffsetSegments,
    IReadOnlyList<decimal> SeamPositionsMillimeters,
    string SeamPositionsText);
