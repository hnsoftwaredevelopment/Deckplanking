using DeckPlanking.Core.Feedback;

namespace DeckPlanking.Core.Tests;

public sealed class FeedbackSubmissionFormatterTests
{
    [Fact]
    public void FormatsBugReportWithApplicationContext()
    {
        var submission = new FeedbackSubmission(
            FeedbackSubmissionType.Bug,
            "Preview is clipped",
            "The bow disappears after changing the rounding.",
            "Henk",
            "henk@example.com",
            new FeedbackApplicationContext(
                "Modelboat Deckplanking",
                "26.06.25.001",
                "Android",
                "Android 15",
                "nl-NL"));

        var body = FeedbackSubmissionFormatter.Format(submission);

        Assert.Contains("Type: Bug", body);
        Assert.Contains("Title: Preview is clipped", body);
        Assert.Contains("App version: 26.06.25.001", body);
        Assert.Contains("Platform: Android", body);
        Assert.Contains("OS version: Android 15", body);
        Assert.Contains("Language: nl-NL", body);
        Assert.Contains("Name: Henk", body);
        Assert.Contains("Contact: henk@example.com", body);
        Assert.Contains("The bow disappears after changing the rounding.", body);
    }

    [Fact]
    public void OmitsEmptyOptionalContactFields()
    {
        var submission = new FeedbackSubmission(
            FeedbackSubmissionType.FeatureRequest,
            "Add print scale",
            "Print the deck at exact scale.",
            string.Empty,
            string.Empty,
            new FeedbackApplicationContext(
                "Modelboat Deckplanking",
                "26.06.25.001",
                "Windows",
                "Windows 11",
                "en-US"));

        var body = FeedbackSubmissionFormatter.Format(submission);

        Assert.Contains("Type: Feature request", body);
        Assert.Contains("Title: Add print scale", body);
        Assert.DoesNotContain("Name:", body);
        Assert.DoesNotContain("Contact:", body);
    }

    [Fact]
    public void FormatsDiagnosticsForUserReview()
    {
        var context = new FeedbackApplicationContext(
            "Modelboat Deckplanking",
            "26.06.30.001",
            "Android",
            "Android 15",
            "nl-NL");
        var diagnostics = new FeedbackDiagnostics(
            "Arm64",
            "Phone",
            "Metric",
            "Saffron",
            "1080x2340 @ 3.0");

        var text = FeedbackDiagnosticsFormatter.Format(context, diagnostics);

        Assert.Contains("App version: 26.06.30.001", text);
        Assert.Contains("Platform: Android", text);
        Assert.Contains("OS version: Android 15", text);
        Assert.Contains("Language: nl-NL", text);
        Assert.Contains("Architecture: Arm64", text);
        Assert.Contains("Device type: Phone", text);
        Assert.Contains("Units: Metric", text);
        Assert.Contains("Theme: Saffron", text);
        Assert.Contains("Screen: 1080x2340 @ 3.0", text);
    }
}
