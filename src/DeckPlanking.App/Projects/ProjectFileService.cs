using DeckPlanking.Core.Projects;

namespace DeckPlanking.App.Projects;

public static partial class ProjectFileService
{
    public static partial Task<ProjectFileResult> SaveAsync(
        DeckPlankingProjectDocument document,
        CancellationToken cancellationToken = default);

    public static partial Task<ProjectFileResult> SaveExistingAsync(
        DeckPlankingProjectDocument document,
        string? filePath,
        CancellationToken cancellationToken = default);

    public static partial Task<ProjectFileResult> SaveNamedAsync(
        DeckPlankingProjectDocument document,
        string projectName,
        CancellationToken cancellationToken = default);

    public static partial Task<ProjectFileResult> RenameAsync(
        string? filePath,
        string projectName,
        CancellationToken cancellationToken = default);

    public static partial Task DeleteAsync(
        string? filePath,
        CancellationToken cancellationToken = default);

    public static partial Task<DeckPlankingProjectDocument?> OpenAsync(
        CancellationToken cancellationToken = default);

    public static partial Task<ProjectOpenResult?> OpenProjectAsync(
        CancellationToken cancellationToken = default);

    private static string BuildSuggestedFileName(DateTimeOffset timestamp)
    {
        return $"deckplanking-project-{timestamp:yyyyMMdd-HHmm}.deckplanking.json";
    }

    private static string BuildProjectFileName(string projectName)
    {
        var sanitizedName = SanitizeProjectName(projectName);
        return sanitizedName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? sanitizedName
            : $"{sanitizedName}.deckplanking.json";
    }

    private static string SanitizeProjectName(string projectName)
    {
        var name = string.Join(
            "_",
            projectName
                .Trim()
                .Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

        return string.IsNullOrWhiteSpace(name)
            ? Path.GetFileNameWithoutExtension(BuildSuggestedFileName(DateTimeOffset.Now))
            : name;
    }
}
