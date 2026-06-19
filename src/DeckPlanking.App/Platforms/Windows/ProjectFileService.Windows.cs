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

        return new ProjectFileResult(true, targetFile.Name, targetFile.Path);
    }

    public static async partial Task<DeckPlankingProjectDocument?> OpenAsync(
        CancellationToken cancellationToken)
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        picker.FileTypeFilter.Add(".json");
        InitializePicker(picker);

        var sourceFile = await picker.PickSingleFileAsync();
        if (sourceFile is null)
        {
            return null;
        }

        var json = await FileIO.ReadTextAsync(sourceFile);
        cancellationToken.ThrowIfCancellationRequested();

        return ProjectJsonSerializer.Deserialize(json);
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
