namespace DeckPlanking.App.Graphics;

public sealed record PreviewSegmentInspection(
    int RenderedRowIndex,
    string RowLabel,
    int SegmentNumber,
    decimal StartMillimeters,
    decimal EndMillimeters)
{
    public decimal LengthMillimeters => EndMillimeters - StartMillimeters;

    public string DisplayText =>
        $"{RowLabel}, segment {SegmentNumber}: {StartMillimeters:0.#}-{EndMillimeters:0.#} mm, length {LengthMillimeters:0.#} mm";
}
