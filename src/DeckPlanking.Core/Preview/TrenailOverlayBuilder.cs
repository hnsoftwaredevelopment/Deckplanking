namespace DeckPlanking.Core.Preview;

public static class TrenailOverlayBuilder
{
    public static IReadOnlyList<TrenailPoint> Build(
        IReadOnlyList<CenterlinePatternPreviewRow> rows,
        decimal? deckLengthMillimeters = null,
        decimal distanceFromPlankEndMillimeters = 4m,
        TrenailPatternKind patternKind = TrenailPatternKind.TwoPerPlankEnd)
    {
        if (rows.Count == 0)
        {
            return [];
        }

        if (patternKind == TrenailPatternKind.None)
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
            var internalSeams = rows[rowIndex].SourceRow.SeamPositionsMillimeters
                .Where(position => position > 0m && position < deckLength)
                .Order()
                .ToArray();

            for (var seamIndex = 0; seamIndex < internalSeams.Length; seamIndex++)
            {
                var position = internalSeams[seamIndex];

                AddPlankEndTrenails(
                    points,
                    rowIndex,
                    position - distanceFromPlankEndMillimeters,
                    deckLength,
                    patternKind,
                    segmentIndex: seamIndex);
                AddPlankEndTrenails(
                    points,
                    rowIndex,
                    position + distanceFromPlankEndMillimeters,
                    deckLength,
                    patternKind,
                    segmentIndex: seamIndex + 1);
            }
        }

        return points;
    }

    private static void AddPlankEndTrenails(
        List<TrenailPoint> points,
        int rowIndex,
        decimal positionMillimeters,
        decimal deckLengthMillimeters,
        TrenailPatternKind patternKind,
        int segmentIndex)
    {
        if (positionMillimeters <= 0m || positionMillimeters >= deckLengthMillimeters)
        {
            return;
        }

        switch (patternKind)
        {
            case TrenailPatternKind.TwoPerPlankEnd:
                points.Add(new TrenailPoint(rowIndex, positionMillimeters, TrenailVerticalPlacement.Upper));
                points.Add(new TrenailPoint(rowIndex, positionMillimeters, TrenailVerticalPlacement.Lower));
                break;
            case TrenailPatternKind.OneCentered:
                points.Add(new TrenailPoint(rowIndex, positionMillimeters, TrenailVerticalPlacement.Center));
                break;
            case TrenailPatternKind.OneAlternating:
                var placement = segmentIndex % 2 == 0
                    ? TrenailVerticalPlacement.Upper
                    : TrenailVerticalPlacement.Lower;
                points.Add(new TrenailPoint(rowIndex, positionMillimeters, placement));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(patternKind), patternKind, null);
        }
    }
}
