namespace DeckPlanking.Core.Preview;

public static class TrenailOverlayBuilder
{
    public static IReadOnlyList<TrenailPoint> Build(
        IReadOnlyList<CenterlinePatternPreviewRow> rows,
        decimal? deckLengthMillimeters = null)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        var deckLength = deckLengthMillimeters ?? rows
            .SelectMany(row => row.SourceRow.SeamPositionsMillimeters)
            .DefaultIfEmpty(0m)
            .Max();

        var points = new List<TrenailPoint>();

        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            foreach (var position in rows[rowIndex].SourceRow.SeamPositionsMillimeters)
            {
                if (position <= 0m || position >= deckLength)
                {
                    continue;
                }

                points.Add(new TrenailPoint(rowIndex, position, TrenailVerticalPlacement.Upper));
                points.Add(new TrenailPoint(rowIndex, position, TrenailVerticalPlacement.Lower));
            }
        }

        return points;
    }
}
