using DeckPlanking.Core.Generation;
using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Tests;

public sealed class DeckPatternGeneratorTests
{
    [Fact]
    public void EveryThreePatternMatchesExcelExample()
    {
        var request = new DeckPatternRequest(
            PlankLengthMillimeters: 141m,
            DeckLengthMillimeters: 600m,
            PatternKind: ShiftPatternKind.Every3,
            RowCount: 3,
            StartPoint: 0);

        var rows = DeckPatternGenerator.Generate(request);

        Assert.Collection(
            rows,
            row =>
            {
                Assert.Equal(1, row.RowNumber);
                Assert.Equal(1, row.Phase);
                Assert.Equal(2, row.SeamOffsetSegments);
                Assert.Equal(new[] { 94m, 235m, 376m, 517m }, row.SeamPositionsMillimeters);
            },
            row =>
            {
                Assert.Equal(2, row.RowNumber);
                Assert.Equal(3, row.Phase);
                Assert.Equal(1, row.SeamOffsetSegments);
                Assert.Equal(new[] { 47m, 188m, 329m, 470m }, row.SeamPositionsMillimeters);
            },
            row =>
            {
                Assert.Equal(3, row.RowNumber);
                Assert.Equal(2, row.Phase);
                Assert.Equal(3, row.SeamOffsetSegments);
                Assert.Equal(new[] { 141m, 282m, 423m, 564m }, row.SeamPositionsMillimeters);
            });
    }

    [Fact]
    public void StartPointWrapsModuloDivisionCount()
    {
        var request = new DeckPatternRequest(
            PlankLengthMillimeters: 141m,
            DeckLengthMillimeters: 200m,
            PatternKind: ShiftPatternKind.Every3,
            RowCount: 1,
            StartPoint: 2);

        var row = Assert.Single(DeckPatternGenerator.Generate(request));

        Assert.Equal(1, row.SeamOffsetSegments);
        Assert.Equal(new[] { 47m, 188m }, row.SeamPositionsMillimeters);
    }

    [Fact]
    public void ZeroOffsetUsesFullDivisionCount()
    {
        var request = new DeckPatternRequest(
            PlankLengthMillimeters: 141m,
            DeckLengthMillimeters: 200m,
            PatternKind: ShiftPatternKind.Every3,
            RowCount: 3,
            StartPoint: 0);

        var rows = DeckPatternGenerator.Generate(request);

        Assert.Equal(3, rows[2].SeamOffsetSegments);
        Assert.Equal(new[] { 141m }, rows[2].SeamPositionsMillimeters);
    }
}
