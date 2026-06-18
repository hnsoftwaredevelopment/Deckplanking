using DeckPlanking.Core.Export;

namespace DeckPlanking.Core.Tests;

public sealed class ExportFileNameBuilderTests
{
    [Fact]
    public void BuildsTimestampedDeckplankingPngFileName()
    {
        var timestamp = new DateTimeOffset(2026, 6, 18, 14, 53, 0, TimeSpan.Zero);

        var fileName = ExportFileNameBuilder.BuildPngFileName(timestamp);

        Assert.Equal("deckplanking-20260618-1453.png", fileName);
    }
}
