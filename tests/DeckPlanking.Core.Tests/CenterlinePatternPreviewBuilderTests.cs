using DeckPlanking.Core.Patterns;
using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class CenterlinePatternPreviewBuilderTests
{
    [Fact]
    public void ContinuesEveryFivePatternAcrossCenterlineWithoutKingPlank()
    {
        var rows = CenterlinePatternPreviewBuilder.Build(
            plankLengthMillimeters: 140m,
            deckLengthMillimeters: 600m,
            patternKind: ShiftPatternKind.Every5,
            rowsPerSide: 8,
            startPoint: 0,
            includeKingPlank: false);

        Assert.Equal([1, 3, 5, 2, 4, 1, 3, 5], rows.Where(row => row.Side == PatternPreviewSide.Upper).Select(row => row.SourceRow.Phase));
        Assert.Equal([2, 4, 1, 3, 5, 2, 4, 1], rows.Where(row => row.Side == PatternPreviewSide.Lower).Select(row => row.SourceRow.Phase));
        Assert.DoesNotContain(rows, row => row.Side == PatternPreviewSide.KingPlank);
    }

    [Fact]
    public void InsertsKingPlankInContinuousEveryFivePattern()
    {
        var rows = CenterlinePatternPreviewBuilder.Build(
            plankLengthMillimeters: 140m,
            deckLengthMillimeters: 600m,
            patternKind: ShiftPatternKind.Every5,
            rowsPerSide: 8,
            startPoint: 0,
            includeKingPlank: true);

        var kingPlank = Assert.Single(rows, row => row.Side == PatternPreviewSide.KingPlank);

        Assert.True(kingPlank.IsKingPlank);
        Assert.Equal(2, kingPlank.SourceRow.Phase);
        Assert.Equal(5, kingPlank.SourceRow.SeamOffsetSegments);
        Assert.Equal([4, 1, 3, 5, 2, 4, 1, 3], rows.Where(row => row.Side == PatternPreviewSide.Lower).Select(row => row.SourceRow.Phase));
    }
}
