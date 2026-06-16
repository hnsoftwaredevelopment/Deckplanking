namespace DeckPlanking.Core.Preview;

public sealed record MirroredPatternPreviewRow(
    PatternPreviewSide Side,
    PatternPreviewRow SourceRow,
    bool IsKingPlank = false);
