using DeckPlanking.App.Graphics;

namespace DeckPlanking.App.Export;

public static partial class PreviewPrinter
{
    public static async Task<PrintResult> PrintAsync(
        DeckPatternPreviewDrawable drawable,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(drawable);

        var fileResult = await PreviewPdfExporter.ExportAsync(drawable, cancellationToken);
        return await PrintPdfAsync(fileResult.FullPath, cancellationToken);
    }

    private static partial Task<PrintResult> PrintPdfAsync(
        string pdfPath,
        CancellationToken cancellationToken);
}

public enum PrintResult
{
    SubmittedToPrintService,
    OpenedPdfForPrinting
}
