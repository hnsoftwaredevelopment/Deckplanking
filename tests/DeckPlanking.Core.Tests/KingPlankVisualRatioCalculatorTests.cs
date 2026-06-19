using DeckPlanking.Core.Configuration;

namespace DeckPlanking.Core.Tests;

public sealed class KingPlankVisualRatioCalculatorTests
{
    [Fact]
    public void CalculatesKingPlankVisualRatio()
    {
        var ratio = KingPlankVisualRatioCalculator.Calculate(
            plankWidthMillimeters: 5m,
            kingPlankWidthMillimeters: 12.5m);

        Assert.Equal(2.5m, ratio);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(5, 0)]
    [InlineData(-1, 5)]
    [InlineData(5, -1)]
    public void RequiresPositiveWidths(decimal plankWidthMillimeters, decimal kingPlankWidthMillimeters)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            KingPlankVisualRatioCalculator.Calculate(
                plankWidthMillimeters,
                kingPlankWidthMillimeters));
    }
}
