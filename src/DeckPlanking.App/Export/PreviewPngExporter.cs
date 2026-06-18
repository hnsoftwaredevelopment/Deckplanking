using DeckPlanking.App.Graphics;
using DeckPlanking.Core.Export;

namespace DeckPlanking.App.Export;

public static class PreviewPngExporter
{
    public static async Task<FileResult> ExportAsync(
        DeckPatternPreviewDrawable drawable,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(drawable);

        var fileName = ExportFileNameBuilder.BuildPngFileName(DateTimeOffset.Now);
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        await using var stream = File.Create(filePath);
        await PreviewImageRenderer.RenderPngAsync(drawable, stream, cancellationToken);

        return new FileResult(filePath)
        {
            ContentType = "image/png"
        };
    }
}
