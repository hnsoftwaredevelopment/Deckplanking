using System.Text;

namespace DeckPlanking.Core.Feedback;

public static class FeedbackDiagnosticsFormatter
{
    public static string Format(FeedbackApplicationContext context, FeedbackDiagnostics diagnostics)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(diagnostics);

        var builder = new StringBuilder();
        builder.AppendLine($"App version: {context.ApplicationVersion}");
        builder.AppendLine($"Platform: {context.Platform}");
        builder.AppendLine($"OS version: {context.OperatingSystemVersion}");
        builder.AppendLine($"Language: {context.Language}");
        builder.AppendLine($"Architecture: {diagnostics.Architecture}");
        builder.AppendLine($"Device type: {diagnostics.DeviceType}");
        builder.AppendLine($"Units: {diagnostics.UnitSystem}");
        builder.AppendLine($"Theme: {diagnostics.Theme}");
        builder.Append($"Screen: {diagnostics.Screen}");
        return builder.ToString();
    }
}
