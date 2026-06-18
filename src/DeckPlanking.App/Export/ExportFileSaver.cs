namespace DeckPlanking.App.Export;

public static partial class ExportFileSaver
{
    public static partial Task<ExportSaveResult> SaveAsync(
        FileResult sourceFile,
        CancellationToken cancellationToken = default);
}
