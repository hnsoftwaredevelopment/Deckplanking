using System.Globalization;
using DeckPlanking.App.Localization;

namespace DeckPlanking.App.Documentation;

public static class UserGuideAssetReader
{
    private static readonly HashSet<string> SupportedLanguages = new(StringComparer.OrdinalIgnoreCase)
    {
        "en",
        "nl",
        "de",
        "fr",
        "es",
        "it",
    };

    public static async Task<string> ReadForCurrentCultureAsync(CancellationToken cancellationToken = default)
    {
        var language = LocalizationResourceManager.Instance.CurrentCulture.TwoLetterISOLanguageName;
        if (!SupportedLanguages.Contains(language))
        {
            language = CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
        }

        var candidates = language.Equals("iv", StringComparison.OrdinalIgnoreCase)
            ? ["en", "nl"]
            : new[] { language, "en", "nl" }.Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var candidate in candidates)
        {
            var assetName = $"UserGuide/user-guide.{candidate}.md";
            try
            {
                await using var stream = await FileSystem.OpenAppPackageFileAsync(assetName);
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync(cancellationToken);
            }
            catch (FileNotFoundException)
            {
            }
        }

        throw new FileNotFoundException("No embedded user guide could be found.");
    }
}
