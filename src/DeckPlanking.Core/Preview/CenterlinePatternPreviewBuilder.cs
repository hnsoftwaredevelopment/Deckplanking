using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Preview;

public static class CenterlinePatternPreviewBuilder
{
    public static IReadOnlyList<CenterlinePatternPreviewRow> Build(
        decimal plankLengthMillimeters,
        decimal deckLengthMillimeters,
        ShiftPatternKind patternKind,
        int rowsPerSide,
        int startPoint,
        bool includeKingPlank)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowsPerSide);

        var totalRows = (rowsPerSide * 2) + (includeKingPlank ? 1 : 0);
        var sourceRows = PatternPreviewBuilder.Build(
            plankLengthMillimeters,
            deckLengthMillimeters,
            patternKind,
            totalRows,
            startPoint);

        var previewRows = new List<CenterlinePatternPreviewRow>(totalRows);

        foreach (var row in sourceRows.Take(rowsPerSide))
        {
            previewRows.Add(new CenterlinePatternPreviewRow(PatternPreviewSide.Upper, row));
        }

        var lowerStartIndex = rowsPerSide;
        if (includeKingPlank)
        {
            var kingPlank = sourceRows[rowsPerSide];
            previewRows.Add(new CenterlinePatternPreviewRow(PatternPreviewSide.KingPlank, kingPlank, IsKingPlank: true));
            lowerStartIndex++;
        }

        foreach (var row in sourceRows.Skip(lowerStartIndex))
        {
            previewRows.Add(new CenterlinePatternPreviewRow(PatternPreviewSide.Lower, row));
        }

        return previewRows;
    }
}
