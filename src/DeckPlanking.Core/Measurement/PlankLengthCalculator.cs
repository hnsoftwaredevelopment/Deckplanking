using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Measurement;

public static class PlankLengthCalculator
{
    public static PlankLengthResult Calculate(PlankLengthRequest request)
    {
        var pattern = ShiftPatternCatalog.Get(request.PatternKind);
        var rawMillimeters = ScaleLengthCalculator.CalculateScaleLengthMillimeters(
            request.RealPlankLength,
            request.RealPlankLengthUnit,
            request.ScaleSettings);

        var cutLength = decimal.Round(rawMillimeters / pattern.DivisionCount, 0, MidpointRounding.AwayFromZero)
            * pattern.DivisionCount;
        var segmentLength = cutLength / pattern.DivisionCount;
        var displayMillimeters = DisplayRounding.RoundMillimeters(cutLength, request.MetricDecimalPlaces);
        var displayInches = DisplayRounding.RoundInchesToFraction(cutLength / 25.4m, request.ImperialFractionDenominator);

        return new PlankLengthResult(rawMillimeters, cutLength, segmentLength, displayMillimeters, displayInches);
    }
}
