using System.Diagnostics;

namespace DeckPlanking.App.Export;

public static partial class PreviewPrinter
{
    private static partial Task PrintPdfAsync(
        string pdfPath,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfPath);

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = pdfPath,
            Verb = "print",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        });

        if (process is null)
        {
            throw new InvalidOperationException("Could not start the Windows print command.");
        }

        return Task.CompletedTask;
    }
}
