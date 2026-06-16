using DeckPlanking.Core.Measurement;
using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Tests;

public sealed class ScaleLengthCalculatorTests
{
    [Fact]
    public void DecimalScaleConvertsRealMetersToScaleMillimeters()
    {
        var settings = ScaleSettings.DecimalScale(64);

        var millimeters = ScaleLengthCalculator.CalculateScaleLengthMillimeters(9m, LengthUnit.Meters, settings);

        Assert.Equal(140.625m, millimeters);
    }

    [Fact]
    public void ImperialScaleConvertsRealFeetToScaleMillimeters()
    {
        var settings = ScaleSettings.ImperialInchesPerFoot(1m / 4m);

        var millimeters = ScaleLengthCalculator.CalculateScaleLengthMillimeters(20m, LengthUnit.Feet, settings);

        Assert.Equal(127m, decimal.Round(millimeters, 3));
    }

    [Fact]
    public void MetricDisplayRoundingDoesNotChangePatternDivision()
    {
        var rounded = DisplayRounding.RoundMillimeters(140.625m, decimalPlaces: 1);
        var pattern = ShiftPatternCatalog.Get(ShiftPatternKind.Every3);

        Assert.Equal(140.6m, rounded);
        Assert.Equal(3, pattern.DivisionCount);
    }

    [Fact]
    public void ImperialDisplayRoundingUsesConfiguredFractionDenominator()
    {
        var rounded = DisplayRounding.RoundInchesToFraction(2.34375m, denominator: 16);

        Assert.Equal(2.375m, rounded);
    }
}
