namespace DeckPlanking.Core.Preview;

public sealed record CenterlinePatternPreviewRow(
    PatternPreviewSide Side,
    PatternPreviewRow SourceRow,
    bool IsKingPlank = false);
