namespace DeckPlanking.Core.Preview;

public sealed record TrenailPoint(
    int RowIndex,
    decimal PositionMillimeters,
    TrenailVerticalPlacement VerticalPlacement);
