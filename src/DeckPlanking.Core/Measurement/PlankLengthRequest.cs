using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Measurement;

public sealed record PlankLengthRequest(
    decimal RealPlankLength,
    LengthUnit RealPlankLengthUnit,
    ScaleSettings ScaleSettings,
    ShiftPatternKind PatternKind,
    int MetricDecimalPlaces,
    int ImperialFractionDenominator);
