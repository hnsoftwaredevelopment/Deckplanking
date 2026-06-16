using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Tests;

public sealed class ShiftPatternTests
{
    [Theory]
    [InlineData(ShiftPatternKind.Every2, new[] { 1, 2 }, 2, 1)]
    [InlineData(ShiftPatternKind.Every3, new[] { 1, 3, 2 }, 3, 2)]
    [InlineData(ShiftPatternKind.Every4, new[] { 1, 3, 2, 4 }, 4, 1)]
    [InlineData(ShiftPatternKind.Every5, new[] { 1, 3, 5, 2, 4 }, 5, 2)]
    public void StandardPatternsExposeExcelSequences(
        ShiftPatternKind kind,
        int[] expectedPhases,
        int expectedDivisionCount,
        int expectedReferencePhase)
    {
        var pattern = ShiftPatternCatalog.Get(kind);

        Assert.Equal(expectedPhases, pattern.Phases);
        Assert.Equal(expectedDivisionCount, pattern.DivisionCount);
        Assert.Equal(expectedReferencePhase, pattern.ReferencePhase);
    }

    [Fact]
    public void VersionOneDoesNotExposeCustomPattern()
    {
        Assert.DoesNotContain(ShiftPatternCatalog.All, pattern => pattern.DisplayName.Contains("Custom"));
    }
}
