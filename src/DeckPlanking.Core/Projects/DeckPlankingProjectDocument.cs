namespace DeckPlanking.Core.Projects;

public sealed record DeckPlankingProjectDocument(
    int SchemaVersion,
    DateTimeOffset SavedAt,
    DeckPlankingProjectSettings Settings);
