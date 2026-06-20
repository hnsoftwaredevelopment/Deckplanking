namespace DeckPlanking.App.Graphics;

public sealed record PreviewSegmentInspection(
    int RenderedRowIndex,
    string RowLabel,
    int SegmentNumber,
    decimal StartMillimeters,
    decimal EndMillimeters)
{
    public decimal LengthMillimeters => EndMillimeters - StartMillimeters;
}
