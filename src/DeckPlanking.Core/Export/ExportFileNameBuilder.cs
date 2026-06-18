namespace DeckPlanking.Core.Export;

public static class ExportFileNameBuilder
{
    public static string BuildPngFileName(DateTimeOffset timestamp)
    {
        return $"deckplanking-{timestamp:yyyyMMdd-HHmm}.png";
    }
}
