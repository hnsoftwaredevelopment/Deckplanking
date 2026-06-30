namespace DeckPlanking.App.Projects;

public sealed record ProjectFileResult(
    bool Saved,
    string FileName,
    string DisplayLocation,
    string? FilePath = null);
