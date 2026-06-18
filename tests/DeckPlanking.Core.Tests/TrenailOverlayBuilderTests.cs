using DeckPlanking.Core.Patterns;
using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class TrenailOverlayBuilderTests
{
    [Fact]
    public void BuildsTwoTrenailsOnBothSidesOfEachInternalSeam()
    {
        var rows = CenterlinePatternPreviewBuilder.Build(
            plankLengthMillimeters: 140m,
            deckLengthMillimeters: 300m,
            patternKind: ShiftPatternKind.Every5,
            rowsPerSide: 1,
            startPoint: 0,
            includeKingPlank: false);

        var points = TrenailOverlayBuilder.Build(rows, deckLengthMillimeters: 300m, distanceFromPlankEndMillimeters: 2m);
        var firstRowSeams = rows[0].SourceRow.SeamPositionsMillimeters
            .Where(position => position > 0m && position < 300m)
            .ToArray();
        var firstSeam = firstRowSeams[0];

        Assert.Equal(firstRowSeams.Length * 4 * rows.Count, points.Count);
        Assert.Contains(points, point => point.RowIndex == 0 && point.PositionMillimeters == firstSeam - 2m && point.VerticalPlacement == TrenailVerticalPlacement.Upper);
        Assert.Contains(points, point => point.RowIndex == 0 && point.PositionMillimeters == firstSeam - 2m && point.VerticalPlacement == TrenailVerticalPlacement.Lower);
        Assert.Contains(points, point => point.RowIndex == 0 && point.PositionMillimeters == firstSeam + 2m && point.VerticalPlacement == TrenailVerticalPlacement.Upper);
        Assert.Contains(points, point => point.RowIndex == 0 && point.PositionMillimeters == firstSeam + 2m && point.VerticalPlacement == TrenailVerticalPlacement.Lower);
    }

    [Fact]
    public void ReturnsNoPointsWhenRowsAreEmpty()
    {
        var points = TrenailOverlayBuilder.Build([]);

        Assert.Empty(points);
    }

    [Fact]
    public void SkipsDeckBoundarySeams()
    {
        var row = new CenterlinePatternPreviewRow(
            PatternPreviewSide.Upper,
            new PatternPreviewRow(
                RowNumber: 1,
                Phase: 1,
                SeamOffsetSegments: 1,
                SeamPositionsMillimeters: [0m, 50m, 100m],
                SeamPositionsText: "0, 50, 100"));

        var points = TrenailOverlayBuilder.Build([row], deckLengthMillimeters: 100m, distanceFromPlankEndMillimeters: 2m);

        Assert.Equal(4, points.Count);
        Assert.DoesNotContain(points, point => point.PositionMillimeters is 0m or 100m);
        Assert.Contains(points, point => point.PositionMillimeters == 48m);
        Assert.Contains(points, point => point.PositionMillimeters == 52m);
    }
}
