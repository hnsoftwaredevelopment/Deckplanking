using Android.Content;
using Android.Provider;
using DeckPlanking.App.Projects;
using DeckPlanking.Core.Projects;
using System.Runtime.Versioning;
using AndroidUri = Android.Net.Uri;

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

    public static async partial Task<ProjectFileResult> SaveExistingAsync(
        DeckPlankingProjectDocument document,
        string? filePath,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (!OperatingSystem.IsAndroidVersionAtLeast(29) || string.IsNullOrWhiteSpace(filePath))
        {
            return await SaveAsync(document, cancellationToken);
        }

        return await SaveToExistingUriAsync(document, filePath, cancellationToken);
    }

    public static async partial Task<ProjectFileResult> SaveNamedAsync(
        DeckPlankingProjectDocument document,
        string projectName,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            throw new PlatformNotSupportedException("Saving projects to Downloads requires Android 10 or newer.");
        }

        return await SaveToDownloadsAsync(document, cancellationToken, BuildProjectFileName(projectName));
    }

    public static async partial Task<ProjectFileResult> RenameAsync(
        string? filePath,
        string projectName,
        CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            throw new PlatformNotSupportedException("Renaming projects requires Android 10 or newer.");
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new InvalidOperationException("Save the project before renaming it.");
        }

        var resolver = GetContentResolver();
        var uri = ParseProjectUri(filePath);
        var fileName = BuildProjectFileName(projectName);

        using var values = new ContentValues();
        values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
        resolver.Update(uri, values, null, null);
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
        return new ProjectFileResult(true, fileName, "Downloads", filePath);
    }

    public static async partial Task DeleteAsync(
        string? filePath,
        CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            throw new PlatformNotSupportedException("Deleting projects requires Android 10 or newer.");
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new InvalidOperationException("This project has not been saved by this app yet.");
        }

        var resolver = GetContentResolver();
        resolver.Delete(ParseProjectUri(filePath), null, null);
        cancellationToken.ThrowIfCancellationRequested();
        await Task.CompletedTask;
    }

    public static async partial Task<DeckPlankingProjectDocument?> OpenAsync(
        CancellationToken cancellationToken)
    {
        var openResult = await OpenProjectAsync(cancellationToken);
        return openResult?.Document;
    }

    public static async partial Task<ProjectOpenResult?> OpenProjectAsync(
        CancellationToken cancellationToken)
    {
        var pickOptions = new PickOptions
        {
            PickerTitle = "Open Deckplanking project",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                [DevicePlatform.Android] = ["application/json", "text/json", "text/plain", "*/*"],
                [DevicePlatform.WinUI] = [".json"]
            })
        };

        var fileResult = await FilePicker.Default.PickAsync(pickOptions);
        if (fileResult is null)
        {
            return null;
        }

        if (!fileResult.FileName.EndsWith(".deckplanking.json", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("Select a Deckplanking project file (*.deckplanking.json).");
        }

        await using var stream = await fileResult.OpenReadAsync();
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync(cancellationToken);
        return new ProjectOpenResult(
            ProjectJsonSerializer.Deserialize(json),
            fileResult.FileName,
            fileResult.FullPath,
            null);
    }

    [SupportedOSPlatform("android29.0")]
    private static async Task<ProjectFileResult> SaveToDownloadsAsync(
        DeckPlankingProjectDocument document,
        CancellationToken cancellationToken)
    {
        return await SaveToDownloadsAsync(
            document,
            cancellationToken,
            BuildSuggestedFileName(DateTimeOffset.Now));
    }

    [SupportedOSPlatform("android29.0")]
    private static async Task<ProjectFileResult> SaveToDownloadsAsync(
        DeckPlankingProjectDocument document,
        CancellationToken cancellationToken,
        string fileName)
    {
        var resolver = GetContentResolver();

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

        return new ProjectFileResult(true, fileName, "Downloads", targetUri.ToString());
    }

    [SupportedOSPlatform("android29.0")]
    private static async Task<ProjectFileResult> SaveToExistingUriAsync(
        DeckPlankingProjectDocument document,
        string filePath,
        CancellationToken cancellationToken)
    {
        var resolver = GetContentResolver();
        var targetUri = ParseProjectUri(filePath);

        await using var targetStream = resolver.OpenOutputStream(targetUri, "wt")
            ?? throw new IOException("Could not open the project file for writing.");
        await using var writer = new StreamWriter(targetStream);
        await writer.WriteAsync(ProjectJsonSerializer.Serialize(document));
        await writer.FlushAsync(cancellationToken);

        return new ProjectFileResult(true, string.Empty, string.Empty, filePath);
    }

    private static ContentResolver GetContentResolver()
    {
        return Platform.CurrentActivity?.ContentResolver
            ?? Android.App.Application.Context.ContentResolver
            ?? throw new InvalidOperationException("Android content resolver is not available.");
    }

    private static AndroidUri ParseProjectUri(string filePath)
    {
        return AndroidUri.Parse(filePath)
            ?? throw new InvalidOperationException("Project file location is not available.");
    }
}
