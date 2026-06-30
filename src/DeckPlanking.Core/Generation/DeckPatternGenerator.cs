using DeckPlanking.Core.Patterns;

namespace DeckPlanking.Core.Generation;

public static class DeckPatternGenerator
{
    public static IReadOnlyList<PlankRow> Generate(DeckPatternRequest request)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.PlankLengthMillimeters);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.DeckLengthMillimeters);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(request.RowCount);

        var pattern = ShiftPatternCatalog.Get(request.PatternKind);
        var segmentLength = request.PlankLengthMillimeters / pattern.DivisionCount;
        var rows = new List<PlankRow>(request.RowCount);

        for (var rowNumber = 1; rowNumber <= request.RowCount; rowNumber++)
        {
            var phase = pattern.Phases[(rowNumber - 1) % pattern.Phases.Count];
            var offset = PositiveModulo(phase - pattern.ReferencePhase + request.StartPoint, pattern.DivisionCount);
            if (offset == 0)
            {
                offset = pattern.DivisionCount;
            }

            rows.Add(new PlankRow(rowNumber, phase, offset, BuildSeams(offset, pattern.DivisionCount, segmentLength, request.DeckLengthMillimeters)));
        }

        return rows;
    }

    private static IReadOnlyList<decimal> BuildSeams(int offset, int divisionCount, decimal segmentLength, decimal deckLengthMillimeters)
    {
        var seams = new List<decimal>();

        for (var seamSegment = offset; ; seamSegment += divisionCount)
        {
            var position = seamSegment * segmentLength;
            if (position > deckLengthMillimeters)
            {
                break;
            }

            seams.Add(position);
        }

        return seams;
    }

    private static int PositiveModulo(int value, int divisor)
    {
        return ((value % divisor) + divisor) % divisor;
    }
}
