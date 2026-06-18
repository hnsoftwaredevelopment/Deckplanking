using Windows.Storage.Pickers;
using WinRT.Interop;

namespace DeckPlanking.App.Export;

public static partial class ExportFileSaver
{
    public static async partial Task<ExportSaveResult> SaveAsync(
        FileResult sourceFile,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sourceFile);

        var fileName = Path.GetFileName(sourceFile.FullPath);
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.Downloads,
            SuggestedFileName = Path.GetFileNameWithoutExtension(fileName),
            DefaultFileExtension = ".png"
        };
        picker.FileTypeChoices.Add("PNG image", [".png"]);

        var window = Application.Current?.Windows.FirstOrDefault()?.Handler?.PlatformView
            as Microsoft.UI.Xaml.Window;
        if (window is not null)
        {
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(window));
        }

        var targetFile = await picker.PickSaveFileAsync();
        if (targetFile is null)
        {
            return new ExportSaveResult(false, fileName, string.Empty);
        }

        await using var sourceStream = File.OpenRead(sourceFile.FullPath);
        await using var targetStream = await targetFile.OpenStreamForWriteAsync();
        targetStream.SetLength(0);
        await sourceStream.CopyToAsync(targetStream, cancellationToken);
        await targetStream.FlushAsync(cancellationToken);

        return new ExportSaveResult(true, targetFile.Name, targetFile.Path);
    }
}
