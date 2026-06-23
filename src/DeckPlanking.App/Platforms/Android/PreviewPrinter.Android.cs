using Android.Content;
using Android.Print;

namespace DeckPlanking.App.Export;

public static partial class PreviewPrinter
{
    private static partial Task<PrintResult> PrintPdfAsync(
        string pdfPath,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfPath);

        var context = Platform.CurrentActivity ?? Android.App.Application.Context;
        var printManager = (PrintManager?)context.GetSystemService(Context.PrintService)
            ?? throw new InvalidOperationException("Android print service is not available.");

        var mediaSize = PrintAttributes.MediaSize.IsoA4?.AsLandscape()
            ?? PrintAttributes.MediaSize.UnknownLandscape!;

        printManager.Print(
            Path.GetFileNameWithoutExtension(pdfPath),
            new PdfPrintDocumentAdapter(pdfPath),
            new PrintAttributes.Builder()
                .SetMediaSize(mediaSize)
                .SetColorMode(PrintColorMode.Color)
                .Build());

        return Task.FromResult(PrintResult.SubmittedToPrintService);
    }
}
