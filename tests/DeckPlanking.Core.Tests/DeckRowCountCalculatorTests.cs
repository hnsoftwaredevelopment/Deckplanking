using DeckPlanking.Core.Configuration;

namespace DeckPlanking.Core.Tests;

public sealed class DeckRowCountCalculatorTests
{
    [Fact]
    public void CalculatesRowsPerSideWithoutKingPlank()
    {
        var rowsPerSide = DeckRowCountCalculator.CalculateRowsPerSide(
            deckWidthMillimeters: 82m,
            plankWidthMillimeters: 5m,
            useKingPlank: false);

        Assert.Equal(9, rowsPerSide);
    }

    [Fact]
    public void CalculatesRowsPerSideWithKingPlank()
    {
        var rowsPerSide = DeckRowCountCalculator.CalculateRowsPerSide(
            deckWidthMillimeters: 85m,
            plankWidthMillimeters: 5m,
            useKingPlank: true);

        Assert.Equal(8, rowsPerSide);
    }

    [Fact]
    public void CalculatesRowsPerSideWithWiderKingPlank()
    {
        var rowsPerSide = DeckRowCountCalculator.CalculateRowsPerSide(
            deckWidthMillimeters: 85m,
            plankWidthMillimeters: 5m,
            useKingPlank: true,
            kingPlankWidthMillimeters: 15m);

        Assert.Equal(7, rowsPerSide);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(80, 0)]
    [InlineData(-1, 5)]
    [InlineData(80, -1)]
    public void RequiresPositiveDimensions(decimal deckWidthMillimeters, decimal plankWidthMillimeters)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DeckRowCountCalculator.CalculateRowsPerSide(
                deckWidthMillimeters,
                plankWidthMillimeters,
                useKingPlank: false));
    }
}
