using System.Text.Json;

namespace DeckPlanking.Core.Feedback;

public static class FeedbackWorkerPayloadBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
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
                submission.ApplicationContext.Language));

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
        FeedbackWorkerContext Context);

    private sealed record FeedbackWorkerContext(
        string AppVersion,
        string Platform,
        string OsVersion,
        string Language);
}
