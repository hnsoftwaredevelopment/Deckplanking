namespace DeckPlanking.Core.Projects;

public sealed record DeckPlankingProjectDocument(
    int SchemaVersion,
    DateTimeOffset SavedAt,
    DeckPlankingProjectSettings Settings)
{
    public static DeckPlankingProjectDocument Create(
        DeckPlankingProjectSettings settings,
        DateTimeOffset savedAt)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return new DeckPlankingProjectDocument(1, savedAt, settings);
    }
}
