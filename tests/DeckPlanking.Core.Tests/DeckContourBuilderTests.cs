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

        Assert.Equal(new DeckContourPoint(0m, 0m), contour[0]);
        Assert.Equal(new DeckContourPoint(0.75m, 0m), contour[1]);
        Assert.Equal(new DeckContourPoint(1m, 0.2m), contour[13]);
        Assert.Equal(new DeckContourPoint(1m, 0.8m), contour[14]);
        Assert.Equal(new DeckContourPoint(0.75m, 1m), contour[26]);
        Assert.Equal(new DeckContourPoint(0m, 1m), contour[^1]);
        Assert.Equal(28, contour.Count);
    }

    [Fact]
    public void BuildsMirroredRoundedBowCurve()
    {
        var contour = DeckContourBuilder.Build(new DeckContourSettings(
            DeckShapeKind.NarrowedBow,
            BowWidthPercentage: 20m,
            SternWidthPercentage: 100m,
            BowTaperLengthPercentage: 25m,
            SternTaperLengthPercentage: 10m,
            BowRoundnessPercentage: 80m));

        var upperCurve = contour
            .Skip(2)
            .Take(12)
            .ToArray();
        var lowerCurve = contour
            .Skip(14)
            .Take(13)
            .ToArray();

        Assert.Equal(new DeckContourPoint(1m, 0.6m), lowerCurve[0]);
        for (var index = 0; index < upperCurve.Length; index++)
        {
            var upperPoint = upperCurve[index];
            var lowerPoint = lowerCurve[^(index + 2)];

            Assert.Equal(upperPoint.XRatio, lowerPoint.XRatio, precision: 12);
            Assert.Equal(1m - upperPoint.YRatio, lowerPoint.YRatio, precision: 12);
        }
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

    [Fact]
    public void KeepsStraightSternContourWhenRoundnessIsZero()
    {
        var contour = DeckContourBuilder.Build(new DeckContourSettings(
            DeckShapeKind.NarrowedBowAndStern,
            BowWidthPercentage: 50m,
            SternWidthPercentage: 80m,
            BowTaperLengthPercentage: 30m,
            SternTaperLengthPercentage: 10m,
            SternRoundnessPercentage: 0m));

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

    [Fact]
    public void BuildsMirroredRoundedSternCurve()
    {
        var contour = DeckContourBuilder.Build(new DeckContourSettings(
            DeckShapeKind.NarrowedBowAndStern,
            BowWidthPercentage: 50m,
            SternWidthPercentage: 60m,
            BowTaperLengthPercentage: 30m,
            SternTaperLengthPercentage: 10m,
            SternRoundnessPercentage: 80m));

        var upperCurve = contour
            .Skip(1)
            .Take(12)
            .ToArray();
        var lowerCurve = contour
            .Skip(contour.Count - 13)
            .Take(13)
            .ToArray();

        Assert.Equal(new DeckContourPoint(0.1m, 0m), upperCurve[^1]);
        Assert.Equal(new DeckContourPoint(0m, 0.8m), lowerCurve[^1]);
        for (var index = 0; index < upperCurve.Length; index++)
        {
            var upperPoint = upperCurve[index];
            var lowerPoint = lowerCurve[^(index + 2)];

            Assert.Equal(upperPoint.XRatio, lowerPoint.XRatio, precision: 12);
            Assert.Equal(1m - upperPoint.YRatio, lowerPoint.YRatio, precision: 12);
        }
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

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void RejectsInvalidSternRoundnessPercentages(decimal percentage)
    {
        var settings = new DeckContourSettings(
            DeckShapeKind.NarrowedBowAndStern,
            BowWidthPercentage: 60m,
            SternWidthPercentage: 90m,
            SternRoundnessPercentage: percentage);

        Assert.Throws<ArgumentOutOfRangeException>(() => DeckContourBuilder.Build(settings));
    }
}
