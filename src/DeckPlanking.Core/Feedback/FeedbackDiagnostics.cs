namespace DeckPlanking.Core.Feedback;

public sealed record FeedbackDiagnostics(
    string Architecture,
    string DeviceType,
    string UnitSystem,
    string Theme,
    string Screen);
