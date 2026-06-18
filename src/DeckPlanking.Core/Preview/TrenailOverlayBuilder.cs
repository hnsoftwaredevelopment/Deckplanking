namespace DeckPlanking.Core.Preview;

public static class TrenailOverlayBuilder
{
    public const decimal DefaultDistanceFromPlankEndMillimeters = 4m;

    public static IReadOnlyList<TrenailPoint> Build(
        IReadOnlyList<CenterlinePatternPreviewRow> rows,
        decimal? deckLengthMillimeters = null,
        decimal distanceFromPlankEndMillimeters = DefaultDistanceFromPlankEndMillimeters,
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
                    plankEnd: PlankEnd.End);
                AddPlankEndTrenails(
                    points,
                    rowIndex,
                    position + distanceFromPlankEndMillimeters,
                    deckLength,
                    patternKind,
                    plankEnd: PlankEnd.Begin);
            }
        }

        return points;
    }

    public static decimal CalculateReadableDistanceFromPlankEnd(
        decimal deckLengthMillimeters,
        decimal renderedDeckWidth,
        decimal minimumRenderedDistance,
        decimal preferredDistanceMillimeters = DefaultDistanceFromPlankEndMillimeters)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(deckLengthMillimeters);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(renderedDeckWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(minimumRenderedDistance);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(preferredDistanceMillimeters);

        var minimumDistanceMillimeters = minimumRenderedDistance * deckLengthMillimeters / renderedDeckWidth;
        return Math.Max(preferredDistanceMillimeters, minimumDistanceMillimeters);
    }

    private static void AddPlankEndTrenails(
        List<TrenailPoint> points,
        int rowIndex,
        decimal positionMillimeters,
        decimal deckLengthMillimeters,
        TrenailPatternKind patternKind,
        PlankEnd plankEnd)
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
                var placement = plankEnd == PlankEnd.End
                    ? TrenailVerticalPlacement.Upper
                    : TrenailVerticalPlacement.Lower;
                points.Add(new TrenailPoint(rowIndex, positionMillimeters, placement));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(patternKind), patternKind, null);
        }
    }

    private enum PlankEnd
    {
        Begin,
        End
    }
}
