using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class RulerTickBuilderTests
{
    [Fact]
    public void BuildsTicksAtEachSegmentBoundaryInsideDeckLength()
    {
        var ticks = RulerTickBuilder.Build(
            segmentLengthMillimeters: 28m,
            deckLengthMillimeters: 100m);

        Assert.Collection(
            ticks,
            tick =>
            {
                Assert.Equal(28m, tick.PositionMillimeters);
                Assert.Equal("28", tick.Label);
            },
            tick =>
            {
                Assert.Equal(56m, tick.PositionMillimeters);
                Assert.Equal("56", tick.Label);
            },
            tick =>
            {
                Assert.Equal(84m, tick.PositionMillimeters);
                Assert.Equal("84", tick.Label);
            });
    }
}
