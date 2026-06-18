using DeckPlanking.App.Graphics;
using Microsoft.Maui.Graphics.Skia;

namespace DeckPlanking.App.Export;

public static class PreviewImageRenderer
{
    public const int ExportWidth = 1600;
    public const int ExportHeight = 900;

    private const float DisplayScale = 1f;
    private const int Dpi = 144;

    public static async Task RenderPngAsync(
        DeckPatternPreviewDrawable drawable,
        Stream targetStream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(drawable);
        ArgumentNullException.ThrowIfNull(targetStream);

        var exportDrawable = drawable.CreateExportSnapshot();
        using var exportContext = new SkiaBitmapExportContext(
            ExportWidth,
            ExportHeight,
            DisplayScale,
            Dpi,
            disposeBitmap: true,
            transparent: false);

        exportDrawable.Draw(exportContext.Canvas, new RectF(0, 0, ExportWidth, ExportHeight));
        exportContext.WriteToStream(targetStream);
        await targetStream.FlushAsync(cancellationToken);
    }
}
