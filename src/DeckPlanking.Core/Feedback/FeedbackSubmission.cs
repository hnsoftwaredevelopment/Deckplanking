namespace DeckPlanking.Core.Feedback;

public sealed record FeedbackSubmission(
    FeedbackSubmissionType Type,
    string Title,
    string Description,
    string? Name,
    string? Contact,
    FeedbackApplicationContext ApplicationContext);
