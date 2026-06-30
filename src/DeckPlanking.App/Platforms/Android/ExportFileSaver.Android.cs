using Android.Content;
using Android.Provider;
using System.Runtime.Versioning;

namespace DeckPlanking.App.Export;

public static partial class ExportFileSaver
{
    public static async partial Task<ExportSaveResult> SaveAsync(
        FileResult sourceFile,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);

        var fileName = Path.GetFileName(sourceFile.FullPath);
        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            throw new PlatformNotSupportedException("Saving exports to Downloads requires Android 10 or newer.");
        }

        return await SaveToDownloadsAsync(sourceFile, fileName, cancellationToken);
    }

    [SupportedOSPlatform("android29.0")]
    private static async Task<ExportSaveResult> SaveToDownloadsAsync(
        FileResult sourceFile,
        string fileName,
        CancellationToken cancellationToken)
    {
        var resolver = Platform.CurrentActivity?.ContentResolver
            ?? Android.App.Application.Context.ContentResolver;
        if (resolver is null)
        {
            throw new InvalidOperationException("Android content resolver is not available.");
        }

        using var values = new ContentValues();
        values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
        values.Put(MediaStore.IMediaColumns.MimeType, sourceFile.ContentType ?? "image/png");
        values.Put(MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);

        var targetUri = resolver.Insert(MediaStore.Downloads.ExternalContentUri, values)
            ?? throw new IOException("Could not create the export file in Downloads.");

        try
        {
            await using var sourceStream = File.OpenRead(sourceFile.FullPath);
            await using var targetStream = resolver.OpenOutputStream(targetUri)
                ?? throw new IOException("Could not open the export file for writing.");
            await sourceStream.CopyToAsync(targetStream, cancellationToken);
            await targetStream.FlushAsync(cancellationToken);
        }
        catch
        {
            resolver.Delete(targetUri, null, null);
            throw;
        }

        return new ExportSaveResult(true, fileName, "Downloads");
    }
}
