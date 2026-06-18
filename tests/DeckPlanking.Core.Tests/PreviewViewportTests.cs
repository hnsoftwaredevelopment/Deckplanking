using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class PreviewViewportTests
{
    [Fact]
    public void ZoomInClampsAtMaximum()
    {
        var viewport = new PreviewViewport(3.9, 0, 0).ZoomBy(2);

        Assert.Equal(4, viewport.Zoom);
    }

    [Fact]
    public void ZoomOutClampsAtMinimum()
    {
        var viewport = new PreviewViewport(0.6, 0, 0).ZoomBy(0.5);

        Assert.Equal(0.5, viewport.Zoom);
    }

    [Fact]
    public void PanAddsDelta()
    {
        var viewport = PreviewViewport.Default.PanBy(12, -8);

        Assert.Equal(12, viewport.PanX);
        Assert.Equal(-8, viewport.PanY);
    }

    [Fact]
    public void ResetReturnsDefaultViewport()
    {
        var viewport = new PreviewViewport(2, 20, 30).Reset();

        Assert.Equal(PreviewViewport.Default, viewport);
    }
}
