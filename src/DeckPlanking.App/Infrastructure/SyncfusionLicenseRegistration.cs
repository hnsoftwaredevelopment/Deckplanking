using Syncfusion.Licensing;

namespace DeckPlanking.App.Infrastructure;

public static class SyncfusionLicenseRegistration
{
    private const string EnvironmentVariableName = "DECKPLANKING_SYNCFUSION_LICENSE";
    private const string LocalLicenseFileName = "syncfusion-license.txt";

    public static void Register()
    {
        var license = Environment.GetEnvironmentVariable(EnvironmentVariableName);

        if (string.IsNullOrWhiteSpace(license))
        {
            var localLicensePath = Path.Combine(AppContext.BaseDirectory, LocalLicenseFileName);
            if (File.Exists(localLicensePath))
            {
                license = File.ReadAllText(localLicensePath);
            }
        }

        if (!string.IsNullOrWhiteSpace(license))
        {
            SyncfusionLicenseProvider.RegisterLicense(license.Trim());
        }
    }
}
