namespace DeckPlanking.Core.Preview;

public static class MirroredPatternPreviewBuilder
{
    public static IReadOnlyList<MirroredPatternPreviewRow> Build(
        IReadOnlyList<PatternPreviewRow> sourceRows,
        bool includeKingPlank = false)
    {
        var mirroredRows = new List<MirroredPatternPreviewRow>((sourceRows.Count * 2) + (includeKingPlank ? 1 : 0));

        for (var index = sourceRows.Count - 1; index >= 0; index--)
        {
            mirroredRows.Add(new MirroredPatternPreviewRow(PatternPreviewSide.Upper, sourceRows[index]));
        }

        if (includeKingPlank)
        {
            mirroredRows.Add(new MirroredPatternPreviewRow(
                PatternPreviewSide.KingPlank,
                new PatternPreviewRow(0, 0, [], "-"),
                IsKingPlank: true));
        }

        foreach (var row in sourceRows)
        {
            mirroredRows.Add(new MirroredPatternPreviewRow(PatternPreviewSide.Lower, row));
        }

        return mirroredRows;
    }
}
