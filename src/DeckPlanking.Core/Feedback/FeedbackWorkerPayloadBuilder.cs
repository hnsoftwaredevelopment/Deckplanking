using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeckPlanking.Core.Feedback;

public static class FeedbackWorkerPayloadBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string BuildJson(FeedbackSubmission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        var payload = new FeedbackWorkerPayload(
            FormatType(submission.Type),
            submission.Title.Trim(),
            submission.Description.Trim(),
            NormalizeOptionalText(submission.Name),
            NormalizeOptionalText(submission.Contact),
            new FeedbackWorkerContext(
                submission.ApplicationContext.ApplicationVersion,
                submission.ApplicationContext.Platform,
                submission.ApplicationContext.OperatingSystemVersion,
                submission.ApplicationContext.Language),
            submission.Type == FeedbackSubmissionType.Bug && submission.Diagnostics is not null
                ? new FeedbackWorkerDiagnostics(
                    submission.Diagnostics.Architecture,
                    submission.Diagnostics.DeviceType,
                    submission.Diagnostics.UnitSystem,
                    submission.Diagnostics.Theme,
                    submission.Diagnostics.Screen)
                : null);

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static string FormatType(FeedbackSubmissionType type)
    {
        return type switch
        {
            FeedbackSubmissionType.FeatureRequest => "feature",
            _ => "bug"
        };
    }

    private static string NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }

    private sealed record FeedbackWorkerPayload(
        string Type,
        string Title,
        string Description,
        string Name,
        string Contact,
        FeedbackWorkerContext Context,
        FeedbackWorkerDiagnostics? Diagnostics);

    private sealed record FeedbackWorkerContext(
        string AppVersion,
        string Platform,
        string OsVersion,
        string Language);

    private sealed record FeedbackWorkerDiagnostics(
        string Architecture,
        string DeviceType,
        string UnitSystem,
        string Theme,
        string Screen);
}
