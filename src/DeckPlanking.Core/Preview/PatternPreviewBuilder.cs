using DeckPlanking.Core.Generation;
using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Preview;

public static class PatternPreviewBuilder
{
    public static IReadOnlyList<PatternPreviewRow> Build(
        decimal plankLengthMillimeters,
        decimal deckLengthMillimeters,
        ShiftPatternKind patternKind,
        int rowCount,
        int startPoint)
    {
        var rows = DeckPatternGenerator.Generate(new DeckPatternRequest(
            plankLengthMillimeters,
            deckLengthMillimeters,
            patternKind,
            rowCount,
            startPoint));

        return rows.Select(ToPreviewRow).ToArray();
    }

    private static PatternPreviewRow ToPreviewRow(PlankRow row)
    {
        var seams = row.SeamPositionsMillimeters.Count == 0
            ? "-"
            : string.Join(", ", row.SeamPositionsMillimeters.Select(position => $"{position:0.###}"));

        return new PatternPreviewRow(row.RowNumber, row.Phase, row.SeamOffsetSegments, row.SeamPositionsMillimeters, seams);
    }
}
