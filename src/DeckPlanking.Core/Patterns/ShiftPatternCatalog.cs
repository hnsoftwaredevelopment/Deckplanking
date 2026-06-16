namespace DeckPlanking.Core.Patterns;

public static class ShiftPatternCatalog
{
    private static readonly ShiftPattern[] Patterns =
    [
        new(ShiftPatternKind.Every2, "Every 2", [1, 2]),
        new(ShiftPatternKind.Every3, "Every 3", [1, 3, 2]),
        new(ShiftPatternKind.Every4, "Every 4", [1, 3, 2, 4]),
        new(ShiftPatternKind.Every5, "Every 5", [1, 3, 5, 2, 4])
    ];

    public static IReadOnlyList<ShiftPattern> All => Patterns;

    public static ShiftPattern Get(ShiftPatternKind kind)
    {
        return Patterns.Single(pattern => pattern.Kind == kind);
    }
}
