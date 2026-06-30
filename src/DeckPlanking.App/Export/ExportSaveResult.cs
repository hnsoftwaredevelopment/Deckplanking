namespace DeckPlanking.App.Export;

public sealed record ExportSaveResult(
    bool Saved,
    string FileName,
    string DisplayLocation);
