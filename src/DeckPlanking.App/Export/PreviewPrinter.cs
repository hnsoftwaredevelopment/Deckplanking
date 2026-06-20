using DeckPlanking.App.Graphics;

namespace DeckPlanking.App.Export;

public static partial class PreviewPrinter
{
    public static async Task PrintAsync(
        DeckPatternPreviewDrawable drawable,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(drawable);

        var fileResult = await PreviewPdfExporter.ExportAsync(drawable, cancellationToken);
        await PrintPdfAsync(fileResult.FullPath, cancellationToken);
    }

    private static partial Task PrintPdfAsync(
        string pdfPath,
        CancellationToken cancellationToken);
}
