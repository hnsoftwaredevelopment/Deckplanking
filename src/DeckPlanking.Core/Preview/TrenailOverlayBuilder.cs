namespace DeckPlanking.Core.Preview;

public static class TrenailOverlayBuilder
{
    public static IReadOnlyList<TrenailPoint> Build(
        IReadOnlyList<CenterlinePatternPreviewRow> rows,
        decimal? deckLengthMillimeters = null,
        decimal distanceFromPlankEndMillimeters = 2m)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(distanceFromPlankEndMillimeters);

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

                AddPlankEndTrenails(points, rowIndex, position - distanceFromPlankEndMillimeters, deckLength);
                AddPlankEndTrenails(points, rowIndex, position + distanceFromPlankEndMillimeters, deckLength);
            }
        }

        return points;
    }

    private static void AddPlankEndTrenails(
        List<TrenailPoint> points,
        int rowIndex,
        decimal positionMillimeters,
        decimal deckLengthMillimeters)
    {
        if (positionMillimeters <= 0m || positionMillimeters >= deckLengthMillimeters)
        {
            return;
        }

        points.Add(new TrenailPoint(rowIndex, positionMillimeters, TrenailVerticalPlacement.Upper));
        points.Add(new TrenailPoint(rowIndex, positionMillimeters, TrenailVerticalPlacement.Lower));
    }
}
