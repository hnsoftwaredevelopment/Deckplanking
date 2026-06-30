using DeckPlanking.App.Infrastructure;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace DeckPlanking.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureSyncfusionCore()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        SyncfusionLicenseRegistration.Register();

        return builder.Build();
    }
}
