using Android.Content;
using Android.Provider;
using DeckPlanking.App.Projects;
using DeckPlanking.Core.Projects;
using System.Runtime.Versioning;

namespace DeckPlanking.App.Projects;

public static partial class ProjectFileService
{
    public static async partial Task<ProjectFileResult> SaveAsync(
        DeckPlankingProjectDocument document,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            throw new PlatformNotSupportedException("Saving projects to Downloads requires Android 10 or newer.");
        }

        return await SaveToDownloadsAsync(document, cancellationToken);
    }

    public static async partial Task<DeckPlankingProjectDocument?> OpenAsync(
        CancellationToken cancellationToken)
    {
        var pickOptions = new PickOptions
        {
            PickerTitle = "Open Deckplanking project",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                [DevicePlatform.Android] = ["application/json", "text/json", "text/plain"],
                [DevicePlatform.WinUI] = [".json"]
            })
        };

        var fileResult = await FilePicker.Default.PickAsync(pickOptions);
        if (fileResult is null)
        {
            return null;
        }

        await using var stream = await fileResult.OpenReadAsync();
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync(cancellationToken);
        return ProjectJsonSerializer.Deserialize(json);
    }

    [SupportedOSPlatform("android29.0")]
    private static async Task<ProjectFileResult> SaveToDownloadsAsync(
        DeckPlankingProjectDocument document,
        CancellationToken cancellationToken)
    {
        var fileName = BuildSuggestedFileName(DateTimeOffset.Now);
        var resolver = Platform.CurrentActivity?.ContentResolver
            ?? Android.App.Application.Context.ContentResolver;
        if (resolver is null)
        {
            throw new InvalidOperationException("Android content resolver is not available.");
        }

        using var values = new ContentValues();
        values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
        values.Put(MediaStore.IMediaColumns.MimeType, "application/json");
        values.Put(MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);

        var targetUri = resolver.Insert(MediaStore.Downloads.ExternalContentUri, values)
            ?? throw new IOException("Could not create the project file in Downloads.");

        try
        {
            await using var targetStream = resolver.OpenOutputStream(targetUri)
                ?? throw new IOException("Could not open the project file for writing.");
            await using var writer = new StreamWriter(targetStream);
            await writer.WriteAsync(ProjectJsonSerializer.Serialize(document));
            await writer.FlushAsync(cancellationToken);
        }
        catch
        {
            resolver.Delete(targetUri, null, null);
            throw;
        }

        return new ProjectFileResult(true, fileName, "Downloads");
    }
}
