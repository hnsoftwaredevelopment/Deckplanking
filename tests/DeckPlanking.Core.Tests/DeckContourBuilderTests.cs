using DeckPlanking.Core.Configuration;
using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Tests;

public sealed class DeckContourBuilderTests
{
    [Fact]
    public void BuildsRectangularContour()
    {
        var contour = DeckContourBuilder.Build(DeckContourSettings.Rectangular);

        Assert.Equal(
            [
                new DeckContourPoint(0m, 0m),
                new DeckContourPoint(1m, 0m),
                new DeckContourPoint(1m, 1m),
                new DeckContourPoint(0m, 1m)
            ],
            contour);
    }

    [Fact]
    public void BuildsSymmetricalBowNarrowedContour()
    {
        var contour = DeckContourBuilder.Build(new DeckContourSettings(
            DeckShapeKind.NarrowedBow,
            BowWidthPercentage: 60m,
            SternWidthPercentage: 100m,
            BowTaperLengthPercentage: 25m,
            SternTaperLengthPercentage: 10m));

        Assert.Equal(
            [
                new DeckContourPoint(0m, 0m),
                new DeckContourPoint(0.75m, 0m),
                new DeckContourPoint(1m, 0.2m),
                new DeckContourPoint(1m, 0.8m),
                new DeckContourPoint(0.75m, 1m),
                new DeckContourPoint(0m, 1m)
            ],
            contour);
    }

    [Fact]
    public void KeepsStraightBowContourWhenRoundnessIsZero()
    {
        var contour = DeckContourBuilder.Build(new DeckContourSettings(
            DeckShapeKind.NarrowedBow,
            BowWidthPercentage: 60m,
            SternWidthPercentage: 100m,
            BowTaperLengthPercentage: 25m,
            SternTaperLengthPercentage: 10m,
            BowRoundnessPercentage: 0m));

        Assert.Equal(
            [
                new DeckContourPoint(0m, 0m),
                new DeckContourPoint(0.75m, 0m),
                new DeckContourPoint(1m, 0.2m),
                new DeckContourPoint(1m, 0.8m),
                new DeckContourPoint(0.75m, 1m),
                new DeckContourPoint(0m, 1m)
            ],
            contour);
    }

    [Fact]
    public void BuildsRoundedBowContour()
    {
        var contour = DeckContourBuilder.Build(new DeckContourSettings(
            DeckShapeKind.NarrowedBow,
            BowWidthPercentage: 60m,
            SternWidthPercentage: 100m,
            BowTaperLengthPercentage: 25m,
            SternTaperLengthPercentage: 10m,
            BowRoundnessPercentage: 100m));

        Assert.Equal(
            [
                new DeckContourPoint(0m, 0m),
                new DeckContourPoint(0.75m, 0m),
                new DeckContourPoint(0.859375m, 0.0125m),
                new DeckContourPoint(0.9375m, 0.05m),
                new DeckContourPoint(0.984375m, 0.1125m),
                new DeckContourPoint(1m, 0.2m),
                new DeckContourPoint(0.984375m, 0.8875m),
                new DeckContourPoint(0.9375m, 0.95m),
                new DeckContourPoint(0.859375m, 0.9875m),
                new DeckContourPoint(0.75m, 1m),
                new DeckContourPoint(0m, 1m)
            ],
            contour);
    }

    [Fact]
    public void BuildsSymmetricalBowAndSternNarrowedContour()
    {
        var contour = DeckContourBuilder.Build(new DeckContourSettings(
            DeckShapeKind.NarrowedBowAndStern,
            BowWidthPercentage: 50m,
            SternWidthPercentage: 80m,
            BowTaperLengthPercentage: 30m,
            SternTaperLengthPercentage: 10m));

        Assert.Equal(
            [
                new DeckContourPoint(0m, 0.1m),
                new DeckContourPoint(0.1m, 0m),
                new DeckContourPoint(0.7m, 0m),
                new DeckContourPoint(1m, 0.25m),
                new DeckContourPoint(1m, 0.75m),
                new DeckContourPoint(0.7m, 1m),
                new DeckContourPoint(0.1m, 1m),
                new DeckContourPoint(0m, 0.9m)
            ],
            contour);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(101)]
    public void RejectsInvalidWidthPercentages(decimal percentage)
    {
        var settings = new DeckContourSettings(
            DeckShapeKind.NarrowedBow,
            BowWidthPercentage: percentage,
            SternWidthPercentage: 100m);

        Assert.Throws<ArgumentOutOfRangeException>(() => DeckContourBuilder.Build(settings));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(51)]
    public void RejectsInvalidTaperPercentages(decimal percentage)
    {
        var settings = new DeckContourSettings(
            DeckShapeKind.NarrowedBow,
            BowWidthPercentage: 60m,
            SternWidthPercentage: 100m,
            BowTaperLengthPercentage: percentage);

        Assert.Throws<ArgumentOutOfRangeException>(() => DeckContourBuilder.Build(settings));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void RejectsInvalidBowRoundnessPercentages(decimal percentage)
    {
        var settings = new DeckContourSettings(
            DeckShapeKind.NarrowedBow,
            BowWidthPercentage: 60m,
            SternWidthPercentage: 100m,
            BowRoundnessPercentage: percentage);

        Assert.Throws<ArgumentOutOfRangeException>(() => DeckContourBuilder.Build(settings));
    }
}
