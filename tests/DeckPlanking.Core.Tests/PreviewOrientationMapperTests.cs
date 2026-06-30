using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class PreviewOrientationMapperTests
{
    [Fact]
    public void KeepsSternOriginRatiosWhenSternIsLeft()
    {
        Assert.Equal(0.25m, PreviewOrientationMapper.MapRatio(0.25m, DeckOrientation.SternLeft));
        Assert.Equal(0.25m, PreviewOrientationMapper.UnmapRatio(0.25m, DeckOrientation.SternLeft));
    }

    [Fact]
    public void MirrorsSternOriginRatiosWhenBowIsLeft()
    {
        Assert.Equal(0.75m, PreviewOrientationMapper.MapRatio(0.25m, DeckOrientation.BowLeft));
        Assert.Equal(0.25m, PreviewOrientationMapper.UnmapRatio(0.75m, DeckOrientation.BowLeft));
    }
}
