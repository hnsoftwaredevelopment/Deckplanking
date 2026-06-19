using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeckPlanking.Core.Projects;

public static class ProjectJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public static string Serialize(DeckPlankingProjectDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return JsonSerializer.Serialize(document, Options);
    }

    public static DeckPlankingProjectDocument Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        var document = JsonSerializer.Deserialize<DeckPlankingProjectDocument>(json, Options)
            ?? throw new InvalidDataException("Project file is empty.");

        if (document.SchemaVersion != 1)
        {
            throw new InvalidDataException(
                $"Project schema version {document.SchemaVersion} is not supported.");
        }

        return document;
    }
}
