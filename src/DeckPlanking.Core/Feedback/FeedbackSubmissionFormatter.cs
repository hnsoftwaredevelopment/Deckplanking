using System.Text;

namespace DeckPlanking.Core.Feedback;

public static class FeedbackSubmissionFormatter
{
    public static string Format(FeedbackSubmission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        var builder = new StringBuilder();
        builder.AppendLine($"Type: {FormatType(submission.Type)}");
        builder.AppendLine($"Title: {submission.Title.Trim()}");
        builder.AppendLine();
        builder.AppendLine("Application");
        builder.AppendLine($"App: {submission.ApplicationContext.ApplicationName}");
        builder.AppendLine($"App version: {submission.ApplicationContext.ApplicationVersion}");
        builder.AppendLine($"Platform: {submission.ApplicationContext.Platform}");
        builder.AppendLine($"OS version: {submission.ApplicationContext.OperatingSystemVersion}");
        builder.AppendLine($"Language: {submission.ApplicationContext.Language}");

        if (!string.IsNullOrWhiteSpace(submission.Name))
        {
            builder.AppendLine($"Name: {submission.Name.Trim()}");
        }

        if (!string.IsNullOrWhiteSpace(submission.Contact))
        {
            builder.AppendLine($"Contact: {submission.Contact.Trim()}");
        }

        builder.AppendLine();
        builder.AppendLine("Description");
        builder.AppendLine(submission.Description.Trim());

        return builder.ToString();
    }

    private static string FormatType(FeedbackSubmissionType type)
    {
        return type switch
        {
            FeedbackSubmissionType.Bug => "Bug",
            FeedbackSubmissionType.FeatureRequest => "Feature request",
            _ => type.ToString()
        };
    }
}
