namespace DeckPlanking.App.Feedback;

public sealed record FeedbackSendResult(bool IsSuccess, string? IssueUrl, string? ErrorMessage)
{
    public static FeedbackSendResult Success(string? issueUrl)
    {
        return new FeedbackSendResult(true, issueUrl, null);
    }

    public static FeedbackSendResult Failure(string errorMessage)
    {
        return new FeedbackSendResult(false, null, errorMessage);
    }
}
