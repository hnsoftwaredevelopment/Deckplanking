using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Generation;

public sealed record DeckPatternRequest(
    decimal PlankLengthMillimeters,
    decimal DeckLengthMillimeters,
    ShiftPatternKind PatternKind,
    int RowCount,
    int StartPoint);
