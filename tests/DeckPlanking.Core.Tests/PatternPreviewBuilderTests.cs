using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class PatternPreviewBuilderTests
{
    [Fact]
    public void FormatsPatternRowsForDisplay()
    {
        var rows = PatternPreviewBuilder.Build(
            plankLengthMillimeters: 140m,
            deckLengthMillimeters: 600m,
            patternKind: Patterns.ShiftPatternKind.Every5,
            rowCount: 2,
            startPoint: 0);

        Assert.Collection(
            rows,
            row =>
            {
                Assert.Equal(1, row.RowNumber);
                Assert.Equal(4, row.SeamOffsetSegments);
                Assert.Equal(new[] { 112m, 252m, 392m, 532m }, row.SeamPositionsMillimeters);
                Assert.Equal("112, 252, 392, 532", row.SeamPositionsText);
            },
            row =>
            {
                Assert.Equal(2, row.RowNumber);
                Assert.Equal(1, row.SeamOffsetSegments);
                Assert.Equal(new[] { 28m, 168m, 308m, 448m, 588m }, row.SeamPositionsMillimeters);
                Assert.Equal("28, 168, 308, 448, 588", row.SeamPositionsText);
            });
    }

    [Fact]
    public void UsesDashWhenNoSeamsFitInsideDeckLength()
    {
        var row = Assert.Single(PatternPreviewBuilder.Build(
            plankLengthMillimeters: 140m,
            deckLengthMillimeters: 20m,
            patternKind: Patterns.ShiftPatternKind.Every5,
            rowCount: 1,
            startPoint: 0));

        Assert.Empty(row.SeamPositionsMillimeters);
        Assert.Equal("-", row.SeamPositionsText);
    }
}
