using DeckPlanking.Core.Measurement;

namespace DeckPlanking.Core.Tests;

public sealed class LengthDisplayFormatterTests
{
    [Fact]
    public void FormatsMetricMillimeters()
    {
        var text = LengthDisplayFormatter.FormatMillimeters(140.25m, decimalPlaces: 1);

        Assert.Equal("140.3 mm", text);
    }

    [Theory]
    [InlineData(6.35, "1/4 in")]
    [InlineData(31.75, "1 1/4 in")]
    [InlineData(12.7, "1/2 in")]
    [InlineData(25.4, "1 in")]
    public void FormatsImperialInchesAsFractions(decimal millimeters, string expected)
    {
        var text = LengthDisplayFormatter.FormatInchesFromMillimeters(millimeters, denominator: 16);

        Assert.Equal(expected, text);
    }
}
