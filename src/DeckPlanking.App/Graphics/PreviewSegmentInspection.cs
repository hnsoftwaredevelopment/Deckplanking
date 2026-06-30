using DeckPlanking.Core.Preview;

namespace DeckPlanking.App.Graphics;

public sealed record PreviewSegmentInspection(
    int RenderedRowIndex,
    PatternPreviewSide Side,
    int RowNumber,
    int SegmentNumber,
    decimal StartMillimeters,
    decimal EndMillimeters)
{
    public decimal LengthMillimeters => EndMillimeters - StartMillimeters;
}
