using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class MirroredPatternPreviewBuilderTests
{
    [Fact]
    public void MirrorsRowsAroundCenterline()
    {
        var sourceRows = new[]
        {
            new PatternPreviewRow(1, 1, [10m], "10"),
            new PatternPreviewRow(2, 2, [20m], "20"),
            new PatternPreviewRow(3, 3, [30m], "30")
        };

        var mirroredRows = MirroredPatternPreviewBuilder.Build(sourceRows);

        Assert.Collection(
            mirroredRows,
            row =>
            {
                Assert.Equal(PatternPreviewSide.Upper, row.Side);
                Assert.Equal(3, row.SourceRow.RowNumber);
            },
            row =>
            {
                Assert.Equal(PatternPreviewSide.Upper, row.Side);
                Assert.Equal(2, row.SourceRow.RowNumber);
            },
            row =>
            {
                Assert.Equal(PatternPreviewSide.Upper, row.Side);
                Assert.Equal(1, row.SourceRow.RowNumber);
            },
            row =>
            {
                Assert.Equal(PatternPreviewSide.Lower, row.Side);
                Assert.Equal(1, row.SourceRow.RowNumber);
            },
            row =>
            {
                Assert.Equal(PatternPreviewSide.Lower, row.Side);
                Assert.Equal(2, row.SourceRow.RowNumber);
            },
            row =>
            {
                Assert.Equal(PatternPreviewSide.Lower, row.Side);
                Assert.Equal(3, row.SourceRow.RowNumber);
            });
    }

    [Fact]
    public void InsertsKingPlankBetweenMirroredHalvesWhenEnabled()
    {
        var sourceRows = new[]
        {
            new PatternPreviewRow(1, 1, [10m], "10"),
            new PatternPreviewRow(2, 2, [20m], "20")
        };

        var mirroredRows = MirroredPatternPreviewBuilder.Build(sourceRows, includeKingPlank: true);

        Assert.Collection(
            mirroredRows,
            row => Assert.Equal(PatternPreviewSide.Upper, row.Side),
            row => Assert.Equal(PatternPreviewSide.Upper, row.Side),
            row =>
            {
                Assert.Equal(PatternPreviewSide.KingPlank, row.Side);
                Assert.True(row.IsKingPlank);
                Assert.Equal(0, row.SourceRow.RowNumber);
            },
            row => Assert.Equal(PatternPreviewSide.Lower, row.Side),
            row => Assert.Equal(PatternPreviewSide.Lower, row.Side));
    }
}
