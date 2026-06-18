namespace DeckPlanking.Core.Preview;

public sealed record PreviewViewport(double Zoom, double PanX, double PanY)
{
    private const double MinimumZoom = 0.5;
    private const double MaximumZoom = 4;

    public static PreviewViewport Default { get; } = new(1, 0, 0);

    public PreviewViewport ZoomBy(double factor)
    {
        if (factor <= 0)
        {
            return this;
        }

        return this with { Zoom = Math.Clamp(Zoom * factor, MinimumZoom, MaximumZoom) };
    }

    public PreviewViewport PanBy(double deltaX, double deltaY)
    {
        return this with
        {
            PanX = PanX + deltaX,
            PanY = PanY + deltaY
        };
    }

    public PreviewViewport Reset()
    {
        return Default;
    }
}
