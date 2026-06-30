namespace DeckPlanking.Core.Measurement;

public sealed record PlankLengthResult(
    decimal RawScaleLengthMillimeters,
    decimal CutLengthMillimeters,
    decimal SegmentLengthMillimeters,
    decimal DisplayLengthMillimeters,
    decimal DisplayLengthInches);
