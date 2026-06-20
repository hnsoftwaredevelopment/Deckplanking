namespace DeckPlanking.Core.Preview;

public static class PlankSegmentBuilder
{
    public static IReadOnlyList<PlankSegment> Build(
        PatternPreviewRow row,
        decimal deckLengthMillimeters)
    {
        ArgumentNullException.ThrowIfNull(row);

        if (deckLengthMillimeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(deckLengthMillimeters));
        }

        var boundaries = row.SeamPositionsMillimeters
            .Where(position => position > 0 && position < deckLengthMillimeters)
            .Order()
            .Prepend(0m)
            .Append(deckLengthMillimeters)
            .ToArray();

        return boundaries
            .Zip(boundaries.Skip(1), (start, end) => new { start, end })
            .Select((segment, index) => new PlankSegment(index + 1, segment.start, segment.end))
            .ToArray();
    }
}
