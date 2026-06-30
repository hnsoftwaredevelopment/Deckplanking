using DeckPlanking.Core.Projects;

namespace DeckPlanking.App.Projects;

public static class LastProjectSettingsStore
{
    private const string FileName = "last-project.deckplanking.json";

    private static string FilePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

    public static async Task SaveAsync(
        DeckPlankingProjectSettings settings,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Directory.CreateDirectory(FileSystem.AppDataDirectory);
        var document = DeckPlankingProjectDocument.Create(settings, DateTimeOffset.UtcNow);
        var json = ProjectJsonSerializer.Serialize(document);

        await File.WriteAllTextAsync(FilePath, json, cancellationToken);
    }

    public static async Task<DeckPlankingProjectSettings?> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(FilePath))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(FilePath, cancellationToken);
        return ProjectJsonSerializer.Deserialize(json).Settings;
    }
}
