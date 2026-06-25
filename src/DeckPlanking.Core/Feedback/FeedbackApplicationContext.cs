namespace DeckPlanking.Core.Feedback;

public sealed record FeedbackApplicationContext(
    string ApplicationName,
    string ApplicationVersion,
    string Platform,
    string OperatingSystemVersion,
    string Language);
