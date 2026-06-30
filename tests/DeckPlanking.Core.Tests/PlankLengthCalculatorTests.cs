using DeckPlanking.Core.Measurement;
using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Tests;

public sealed class PlankLengthCalculatorTests
{
    [Fact]
    public void DecimalScaleProducesRoundedCutLengthAndSegmentLength()
    {
        var request = new PlankLengthRequest(
            RealPlankLength: 9m,
            RealPlankLengthUnit: LengthUnit.Meters,
            ScaleSettings: ScaleSettings.DecimalScale(64),
            PatternKind: ShiftPatternKind.Every5,
            MetricDecimalPlaces: 1,
            ImperialFractionDenominator: 16);

        var result = PlankLengthCalculator.Calculate(request);

        Assert.Equal(140.625m, result.RawScaleLengthMillimeters);
        Assert.Equal(140m, result.CutLengthMillimeters);
        Assert.Equal(28m, result.SegmentLengthMillimeters);
        Assert.Equal(140.0m, result.DisplayLengthMillimeters);
    }

    [Fact]
    public void ImperialDisplayLengthIsRoundedIndependentlyFromPatternDivision()
    {
        var request = new PlankLengthRequest(
            RealPlankLength: 20m,
            RealPlankLengthUnit: LengthUnit.Feet,
            ScaleSettings: ScaleSettings.ImperialInchesPerFoot(1m / 4m),
            PatternKind: ShiftPatternKind.Every3,
            MetricDecimalPlaces: 2,
            ImperialFractionDenominator: 16);

        var result = PlankLengthCalculator.Calculate(request);

        Assert.Equal(127m, decimal.Round(result.RawScaleLengthMillimeters, 3));
        Assert.Equal(126m, result.CutLengthMillimeters);
        Assert.Equal(42m, result.SegmentLengthMillimeters);
        Assert.Equal(4.9375m, result.DisplayLengthInches);
    }
}
