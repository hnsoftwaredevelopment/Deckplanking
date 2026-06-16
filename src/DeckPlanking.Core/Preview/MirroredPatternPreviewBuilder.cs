namespace DeckPlanking.Core.Preview;

public static class MirroredPatternPreviewBuilder
{
    public static IReadOnlyList<MirroredPatternPreviewRow> Build(IReadOnlyList<PatternPreviewRow> sourceRows)
    {
        var mirroredRows = new List<MirroredPatternPreviewRow>(sourceRows.Count * 2);

        for (var index = sourceRows.Count - 1; index >= 0; index--)
        {
            mirroredRows.Add(new MirroredPatternPreviewRow(PatternPreviewSide.Upper, sourceRows[index]));
        }

        foreach (var row in sourceRows)
        {
            mirroredRows.Add(new MirroredPatternPreviewRow(PatternPreviewSide.Lower, row));
        }

        return mirroredRows;
    }
}
