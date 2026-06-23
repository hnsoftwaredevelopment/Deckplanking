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

        return await SaveWithPickerAsync(
            document,
            Path.GetFileNameWithoutExtension(BuildSuggestedFileName(DateTimeOffset.Now)),
            cancellationToken);
    }

    public static async partial Task<ProjectFileResult> SaveNamedAsync(
        DeckPlankingProjectDocument document,
        string projectName,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        return await SaveWithPickerAsync(
            document,
            Path.GetFileNameWithoutExtension(BuildProjectFileName(projectName)),
            cancellationToken);
    }

    private static async Task<ProjectFileResult> SaveWithPickerAsync(
        DeckPlankingProjectDocument document,
        string suggestedFileName,
        CancellationToken cancellationToken)
    {
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

    public static async partial Task<ProjectFileResult> RenameAsync(
        string? filePath,
        string projectName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new InvalidOperationException("Save the project before renaming it.");
        }

        var newFileName = BuildProjectFileName(projectName);
        var directory = Path.GetDirectoryName(filePath)
            ?? throw new InvalidOperationException("Project location is not available.");
        var newFilePath = Path.Combine(directory, newFileName);

        if (File.Exists(newFilePath)
            && !string.Equals(filePath, newFilePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new IOException("A project with that name already exists.");
        }

        if (!string.Equals(filePath, newFilePath, StringComparison.OrdinalIgnoreCase))
        {
            File.Move(filePath, newFilePath);
        }

        await Task.CompletedTask;
        cancellationToken.ThrowIfCancellationRequested();

        return new ProjectFileResult(true, newFileName, newFilePath, newFilePath);
    }

    public static async partial Task DeleteAsync(
        string? filePath,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new InvalidOperationException("This project has not been saved yet.");
        }

        File.Delete(filePath);
        await Task.CompletedTask;
        cancellationToken.ThrowIfCancellationRequested();
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
