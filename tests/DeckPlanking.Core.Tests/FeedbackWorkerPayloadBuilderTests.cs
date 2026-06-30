using System.Text.Json;
using DeckPlanking.Core.Feedback;

namespace DeckPlanking.Core.Tests;

public sealed class FeedbackWorkerPayloadBuilderTests
{
    [Fact]
    public void BuildsBugPayloadForWorker()
    {
        var submission = new FeedbackSubmission(
            FeedbackSubmissionType.Bug,
            "Preview is clipped",
            "The bow disappears.",
            "Henk",
            "henk@example.com",
            new FeedbackApplicationContext(
                "Modelboat Deckplanking",
                "26.06.30.001",
                "Android",
                "Android 15",
                "nl-NL"));

        var json = FeedbackWorkerPayloadBuilder.BuildJson(submission);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.Equal("bug", root.GetProperty("type").GetString());
        Assert.Equal("Preview is clipped", root.GetProperty("title").GetString());
        Assert.Equal("The bow disappears.", root.GetProperty("description").GetString());
        Assert.Equal("Henk", root.GetProperty("name").GetString());
        Assert.Equal("henk@example.com", root.GetProperty("contact").GetString());
        Assert.Equal("26.06.30.001", root.GetProperty("context").GetProperty("appVersion").GetString());
        Assert.Equal("Android", root.GetProperty("context").GetProperty("platform").GetString());
        Assert.Equal("Android 15", root.GetProperty("context").GetProperty("osVersion").GetString());
        Assert.Equal("nl-NL", root.GetProperty("context").GetProperty("language").GetString());
    }

    [Fact]
    public void BuildsFeaturePayloadForWorker()
    {
        var submission = new FeedbackSubmission(
            FeedbackSubmissionType.FeatureRequest,
            "Add exact print scale",
            "Print the deck at exact scale.",
            null,
            null,
            new FeedbackApplicationContext(
                "Modelboat Deckplanking",
                "26.06.30.001",
                "Windows",
                "Windows 11",
                "en-US"));

        var json = FeedbackWorkerPayloadBuilder.BuildJson(submission);
        using var document = JsonDocument.Parse(json);

        Assert.Equal("feature", document.RootElement.GetProperty("type").GetString());
        Assert.Equal(string.Empty, document.RootElement.GetProperty("name").GetString());
        Assert.Equal(string.Empty, document.RootElement.GetProperty("contact").GetString());
    }
}
