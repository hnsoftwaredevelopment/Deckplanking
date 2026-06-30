namespace DeckPlanking.Core.Preview;

public static class RulerTickBuilder
{
    public static IReadOnlyList<RulerTick> Build(decimal segmentLengthMillimeters, decimal deckLengthMillimeters)
    {
        if (segmentLengthMillimeters <= 0 || deckLengthMillimeters <= 0)
        {
            return [];
        }

        var ticks = new List<RulerTick>();

        for (var position = segmentLengthMillimeters; position < deckLengthMillimeters; position += segmentLengthMillimeters)
        {
            ticks.Add(new RulerTick(position, $"{position:0.###}"));
        }

        return ticks;
    }
}
