using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DeckPlanking.Core.Feedback;

namespace DeckPlanking.App.Feedback;

public sealed class FeedbackWorkerClient
{
    private static readonly Uri Endpoint = new("https://deckplanking-feedback.herbert-nijkamp.workers.dev/feedback");
    private readonly HttpClient httpClient;

    public FeedbackWorkerClient()
        : this(new HttpClient())
    {
    }

    internal FeedbackWorkerClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<FeedbackSendResult> SendAsync(FeedbackSubmission submission, CancellationToken cancellationToken = default)
    {
        var payload = FeedbackWorkerPayloadBuilder.BuildJson(submission);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint)
        {
            Content = content
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return FeedbackSendResult.Failure(ParseError(body) ?? $"HTTP {(int)response.StatusCode}");
            }

            return FeedbackSendResult.Success(ParseIssueUrl(body));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return FeedbackSendResult.Failure(ex.Message);
        }
    }

    private static string? ParseIssueUrl(string body)
    {
        using var document = JsonDocument.Parse(body);
        return document.RootElement.TryGetProperty("issueUrl", out var issueUrl)
            ? issueUrl.GetString()
            : null;
    }

    private static string? ParseError(string body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            return document.RootElement.TryGetProperty("error", out var error)
                ? error.GetString()
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
