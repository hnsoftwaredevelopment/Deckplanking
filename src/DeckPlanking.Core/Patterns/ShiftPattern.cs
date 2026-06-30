namespace DeckPlanking.Core.Patterns;

public sealed record ShiftPattern(ShiftPatternKind Kind, string DisplayName, IReadOnlyList<int> Phases)
{
    public int DivisionCount => Phases.Max();

    public int ReferencePhase => Phases[(9 - 1) % Phases.Count];
}
