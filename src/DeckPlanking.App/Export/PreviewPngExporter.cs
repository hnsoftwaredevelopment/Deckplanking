using DeckPlanking.App.Graphics;
using DeckPlanking.Core.Export;
using Microsoft.Maui.Graphics.Skia;

namespace DeckPlanking.App.Export;

public static class PreviewPngExporter
{
    private const int ExportWidth = 1600;
    private const int ExportHeight = 900;
    private const float DisplayScale = 1f;
    private const int Dpi = 144;

    public static async Task<FileResult> ExportAsync(
        DeckPatternPreviewDrawable drawable,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(drawable);

        var fileName = ExportFileNameBuilder.BuildPngFileName(DateTimeOffset.Now);
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        var exportDrawable = drawable.CreateExportSnapshot();
        using var exportContext = new SkiaBitmapExportContext(
            ExportWidth,
            ExportHeight,
            DisplayScale,
            Dpi,
            disposeBitmap: true,
            transparent: false);

        exportDrawable.Draw(exportContext.Canvas, new RectF(0, 0, ExportWidth, ExportHeight));

        await using var stream = File.Create(filePath);
        exportContext.WriteToStream(stream);
        await stream.FlushAsync(cancellationToken);

        return new FileResult(filePath)
        {
            ContentType = "image/png"
        };
    }
}
