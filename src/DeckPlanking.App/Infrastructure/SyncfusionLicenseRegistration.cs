using Syncfusion.Licensing;

namespace DeckPlanking.App.Infrastructure;

public static class SyncfusionLicenseRegistration
{
    private const string EnvironmentVariableName = "DECKPLANKING_SYNCFUSION_LICENSE";

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

        if (string.IsNullOrWhiteSpace(license))
        {
            foreach (var localLicenseFileName in LocalLicenseFileNames)
            {
                var localLicensePath = Path.Combine(AppContext.BaseDirectory, localLicenseFileName);
                if (File.Exists(localLicensePath))
                {
                    license = File.ReadAllText(localLicensePath);
                    break;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(license))
        {
            SyncfusionLicenseProvider.RegisterLicense(license.Trim());
        }
    }
}
