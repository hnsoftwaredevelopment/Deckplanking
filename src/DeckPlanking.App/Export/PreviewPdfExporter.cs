using DeckPlanking.App.Graphics;
using DeckPlanking.Core.Export;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace DeckPlanking.App.Export;

public static class PreviewPdfExporter
{
    private const float PageMargin = 36f;

    public static async Task<FileResult> ExportAsync(
        DeckPatternPreviewDrawable drawable,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(drawable);

        var fileName = ExportFileNameBuilder.BuildPdfFileName(DateTimeOffset.Now);
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        await using var pngStream = new MemoryStream();
        await PreviewImageRenderer.RenderPngAsync(drawable, pngStream, cancellationToken);
        pngStream.Position = 0;

        using var document = new PdfDocument();
        document.PageSettings.Orientation = PdfPageOrientation.Landscape;
        document.PageSettings.Margins.All = PageMargin;

        var page = document.Pages.Add();
        using var image = new PdfBitmap(pngStream);
        var drawBounds = CalculateImageBounds(
            page.GetClientSize(),
            PreviewImageRenderer.ExportWidth,
            PreviewImageRenderer.ExportHeight);

        page.Graphics.DrawImage(image, drawBounds);

        await using var pdfStream = File.Create(filePath);
        document.Save(pdfStream);
        await pdfStream.FlushAsync(cancellationToken);

        return new FileResult(filePath)
        {
            ContentType = "application/pdf"
        };
    }

    private static RectangleF CalculateImageBounds(
        Syncfusion.Drawing.SizeF clientSize,
        float imageWidth,
        float imageHeight)
    {
        var scale = Math.Min(clientSize.Width / imageWidth, clientSize.Height / imageHeight);
        var width = imageWidth * scale;
        var height = imageHeight * scale;
        var x = (clientSize.Width - width) / 2f;
        var y = (clientSize.Height - height) / 2f;

        return new RectangleF(x, y, width, height);
    }
}
