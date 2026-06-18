namespace DeckPlanking.Core.Preview;

public static class DirectionGuideBuilder
{
    public static DirectionGuide Build(DeckOrientation orientation)
    {
        return orientation switch
        {
            DeckOrientation.BowLeft => new DirectionGuide("Boeg", "Hek"),
            DeckOrientation.SternLeft => new DirectionGuide("Hek", "Boeg"),
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };
    }
}
