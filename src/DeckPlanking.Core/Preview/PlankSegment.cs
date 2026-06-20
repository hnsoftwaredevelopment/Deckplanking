namespace DeckPlanking.Core.Preview;

public sealed record PlankSegment(
    int SegmentNumber,
    decimal StartMillimeters,
    decimal EndMillimeters)
{
    public decimal LengthMillimeters => EndMillimeters - StartMillimeters;
}
