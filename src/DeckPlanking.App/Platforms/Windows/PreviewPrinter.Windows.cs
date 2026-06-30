using System.ComponentModel;
using System.Diagnostics;

namespace DeckPlanking.App.Export;

public static partial class PreviewPrinter
{
    private static partial Task<PrintResult> PrintPdfAsync(
        string pdfPath,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfPath);

        try
        {
            StartShellProcess(pdfPath, "print");
            return Task.FromResult(PrintResult.SubmittedToPrintService);
        }
        catch (Win32Exception)
        {
            StartShellProcess(pdfPath, verb: null);
            return Task.FromResult(PrintResult.OpenedPdfForPrinting);
        }
    }

    private static void StartShellProcess(string pdfPath, string? verb)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = pdfPath,
            Verb = verb ?? string.Empty,
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        });

        if (process is null)
        {
            throw new InvalidOperationException("Could not start the Windows print command.");
        }
    }
}
