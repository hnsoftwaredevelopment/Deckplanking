using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class DirectionGuideBuilderTests
{
    [Fact]
    public void PlacesBowOnLeftForBowLeftOrientation()
    {
        var guide = DirectionGuideBuilder.Build(DeckOrientation.BowLeft);

        Assert.Equal("Boeg", guide.LeftLabel);
        Assert.Equal("Hek", guide.RightLabel);
    }

    [Fact]
    public void PlacesSternOnLeftForSternLeftOrientation()
    {
        var guide = DirectionGuideBuilder.Build(DeckOrientation.SternLeft);

        Assert.Equal("Hek", guide.LeftLabel);
        Assert.Equal("Boeg", guide.RightLabel);
    }
}
