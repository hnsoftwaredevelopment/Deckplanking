using Syncfusion.Licensing;

namespace DeckPlanking.App.Infrastructure;

public static class SyncfusionLicenseRegistration
{
    private const string EnvironmentVariableName = "DECKPLANKING_SYNCFUSION_LICENSE";

    public static string LastRegistrationSource { get; private set; } = "Not registered";

    private static readonly string[] LocalLicenseFileNames =
    [
        "syncfusion-license.txt",
        "SyncfusionLicense.txt",
        "synfusion.txt",
        "SynfusionLicense.txt"
    ];

    public static void Register()
    {
        var license = Environment.GetEnvironmentVariable(EnvironmentVariableName);
        var registrationSource = EnvironmentVariableName;

        if (string.IsNullOrWhiteSpace(license))
        {
            var localLicensePath = FindLocalLicensePath();
            if (localLicensePath is not null)
            {
                license = File.ReadAllText(localLicensePath);
                registrationSource = localLicensePath;
            }
        }

        if (!string.IsNullOrWhiteSpace(license))
        {
            SyncfusionLicenseProvider.RegisterLicense(license.Trim());
            LastRegistrationSource = registrationSource;
        }
    }

    private static string? FindLocalLicensePath()
    {
        foreach (var searchDirectory in GetSearchDirectories())
        {
            foreach (var localLicenseFileName in LocalLicenseFileNames)
            {
                var localLicensePath = Path.Combine(searchDirectory, localLicenseFileName);
                if (File.Exists(localLicensePath))
                {
                    return localLicensePath;
                }
            }
        }

        return null;
    }

    private static IEnumerable<string> GetSearchDirectories()
    {
        foreach (var baseDirectory in new[] { AppContext.BaseDirectory, Environment.CurrentDirectory })
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                continue;
            }

            yield return baseDirectory;

            var repositoryRoot = FindRepositoryRoot(baseDirectory);
            if (repositoryRoot is null)
            {
                continue;
            }

            yield return repositoryRoot;
            yield return Path.Combine(repositoryRoot, "docs");
        }
    }

    private static string? FindRepositoryRoot(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, ".git"))
                || File.Exists(Path.Combine(directory.FullName, "Deckplanking.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
