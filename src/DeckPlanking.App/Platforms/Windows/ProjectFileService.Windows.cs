using DeckPlanking.App.Projects;
using DeckPlanking.Core.Projects;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace DeckPlanking.App.Projects;

public static partial class ProjectFileService
{
    public static async partial Task<ProjectFileResult> SaveAsync(
        DeckPlankingProjectDocument document,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        var suggestedFileName = Path.GetFileNameWithoutExtension(BuildSuggestedFileName(DateTimeOffset.Now));
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = suggestedFileName,
            DefaultFileExtension = ".json"
        };
        picker.FileTypeChoices.Add("Deckplanking project", [".json"]);
        InitializePicker(picker);

        var targetFile = await picker.PickSaveFileAsync();
        if (targetFile is null)
        {
            return new ProjectFileResult(false, string.Empty, string.Empty);
        }

        var json = ProjectJsonSerializer.Serialize(document);
        await FileIO.WriteTextAsync(targetFile, json);
        cancellationToken.ThrowIfCancellationRequested();

        return new ProjectFileResult(true, targetFile.Name, targetFile.Path, targetFile.Path);
    }

    public static async partial Task<ProjectFileResult> SaveExistingAsync(
        DeckPlankingProjectDocument document,
        string? filePath,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return await SaveAsync(document, cancellationToken);
        }

        var json = ProjectJsonSerializer.Serialize(document);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);

        return new ProjectFileResult(
            true,
            Path.GetFileName(filePath),
            filePath,
            filePath);
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
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        picker.FileTypeFilter.Add("*");
        InitializePicker(picker);

        var sourceFile = await picker.PickSingleFileAsync();
        if (sourceFile is null)
        {
            return null;
        }

        if (!sourceFile.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("Select a Deckplanking project JSON file.");
        }

        var json = await FileIO.ReadTextAsync(sourceFile);
        cancellationToken.ThrowIfCancellationRequested();

        return new ProjectOpenResult(
            ProjectJsonSerializer.Deserialize(json),
            sourceFile.Name,
            sourceFile.Path,
            sourceFile.Path);
    }

    private static void InitializePicker(object picker)
    {
        var window = Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView
            as Microsoft.UI.Xaml.Window;
        if (window is null)
        {
            return;
        }

        InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(window));
    }
}
