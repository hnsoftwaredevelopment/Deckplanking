namespace DeckPlanking.Core.Preview;

public static class PreviewOrientationMapper
{
    public static decimal MapRatio(decimal sternOriginRatio, DeckOrientation orientation)
    {
        return orientation == DeckOrientation.BowLeft
            ? 1m - sternOriginRatio
            : sternOriginRatio;
    }

    public static float MapRatio(float sternOriginRatio, DeckOrientation orientation)
    {
        return orientation == DeckOrientation.BowLeft
            ? 1f - sternOriginRatio
            : sternOriginRatio;
    }

    public static decimal UnmapRatio(decimal displayedRatio, DeckOrientation orientation)
    {
        return orientation == DeckOrientation.BowLeft
            ? 1m - displayedRatio
            : displayedRatio;
    }
}
