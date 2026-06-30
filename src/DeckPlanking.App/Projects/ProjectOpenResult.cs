using DeckPlanking.Core.Projects;

namespace DeckPlanking.App.Projects;

public sealed record ProjectOpenResult(
    DeckPlankingProjectDocument Document,
    string FileName,
    string DisplayLocation,
    string? FilePath = null);
