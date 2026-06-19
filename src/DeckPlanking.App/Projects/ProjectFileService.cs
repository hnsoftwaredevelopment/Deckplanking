using DeckPlanking.Core.Projects;

namespace DeckPlanking.App.Projects;

public static partial class ProjectFileService
{
    public static partial Task<ProjectFileResult> SaveAsync(
        DeckPlankingProjectDocument document,
        CancellationToken cancellationToken = default);

    public static partial Task<DeckPlankingProjectDocument?> OpenAsync(
        CancellationToken cancellationToken = default);

    private static string BuildSuggestedFileName(DateTimeOffset timestamp)
    {
        return $"deckplanking-project-{timestamp:yyyyMMdd-HHmm}.deckplanking.json";
    }
}
