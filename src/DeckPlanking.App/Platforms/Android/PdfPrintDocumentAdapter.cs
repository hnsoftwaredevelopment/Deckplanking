using Android.OS;
using Android.Print;
using Java.IO;
using File = System.IO.File;

namespace DeckPlanking.App.Export;

public sealed class PdfPrintDocumentAdapter(string pdfPath) : PrintDocumentAdapter
{
    public override void OnLayout(
        PrintAttributes? oldAttributes,
        PrintAttributes? newAttributes,
        CancellationSignal? cancellationSignal,
        LayoutResultCallback? callback,
        Bundle? extras)
    {
        if (cancellationSignal?.IsCanceled == true)
        {
            callback?.OnLayoutCancelled();
            return;
        }

        var info = new PrintDocumentInfo.Builder(Path.GetFileName(pdfPath))
            .SetContentType(PrintContentType.Document)
            .Build();

        callback?.OnLayoutFinished(info, changed: true);
    }

    public override void OnWrite(
        PageRange[]? pages,
        ParcelFileDescriptor? destination,
        CancellationSignal? cancellationSignal,
        WriteResultCallback? callback)
    {
        if (destination is null)
        {
            callback?.OnWriteFailed("Print destination is not available.");
            return;
        }

        try
        {
            using var output = new FileOutputStream(destination.FileDescriptor);
            var bytes = File.ReadAllBytes(pdfPath);
            output.Write(bytes);
            output.Flush();

            if (cancellationSignal?.IsCanceled == true)
            {
                callback?.OnWriteCancelled();
                return;
            }

            callback?.OnWriteFinished([PageRange.AllPages!]);
        }
        catch (Exception ex)
        {
            callback?.OnWriteFailed(ex.Message ?? "Printing failed.");
        }
    }
}
