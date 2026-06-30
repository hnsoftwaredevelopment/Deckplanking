using DeckPlanking.Core.Configuration;

namespace DeckPlanking.Core.Preview;

public static class DeckContourBuilder
{
    private const int BowCurveSegments = 12;
    private const int SternCurveSegments = 12;

    public static IReadOnlyList<DeckContourPoint> Build(DeckContourSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        ValidatePercentage(settings.BowWidthPercentage, nameof(settings.BowWidthPercentage));
        ValidatePercentage(settings.SternWidthPercentage, nameof(settings.SternWidthPercentage));
        ValidateTaperPercentage(settings.BowTaperLengthPercentage, nameof(settings.BowTaperLengthPercentage));
        ValidateTaperPercentage(settings.SternTaperLengthPercentage, nameof(settings.SternTaperLengthPercentage));
        ValidateRoundnessPercentage(settings.BowRoundnessPercentage, nameof(settings.BowRoundnessPercentage));
        ValidateRoundnessPercentage(settings.SternRoundnessPercentage, nameof(settings.SternRoundnessPercentage));

        var sternWidth = settings.Shape == DeckShapeKind.NarrowedBowAndStern
            ? settings.SternWidthPercentage / 100m
            : 1m;
        var bowWidth = settings.Shape == DeckShapeKind.Rectangular
            ? 1m
            : settings.BowWidthPercentage / 100m;

        var sternInset = (1m - sternWidth) / 2m;
        var bowInset = (1m - bowWidth) / 2m;
        var sternTaperEnd = settings.Shape == DeckShapeKind.NarrowedBowAndStern
            ? settings.SternTaperLengthPercentage / 100m
            : 0m;
        var bowTaperStart = settings.Shape == DeckShapeKind.Rectangular
            ? 1m
            : 1m - (settings.BowTaperLengthPercentage / 100m);

        var points = new List<DeckContourPoint>
        {
            new(0m, sternInset)
        };

        if (settings.Shape == DeckShapeKind.NarrowedBowAndStern && settings.SternRoundnessPercentage > 0m)
        {
            points.AddRange(BuildCurve(
                new DeckContourPoint(0m, sternInset),
                new DeckContourPoint(sternTaperEnd, 0m),
                new DeckContourPoint(0m, 0m),
                settings.SternRoundnessPercentage,
                SternCurveSegments));
        }
        else
        {
            points.Add(new(sternTaperEnd, 0m));
        }

        points.Add(new(bowTaperStart, 0m));

        if (settings.Shape == DeckShapeKind.Rectangular || settings.BowRoundnessPercentage == 0m)
        {
            points.Add(new(1m, bowInset));
            points.Add(new(1m, 1m - bowInset));
            points.Add(new(bowTaperStart, 1m));
        }
        else
        {
            var upperBowFront = new DeckContourPoint(1m, bowInset);
            var lowerBowFront = new DeckContourPoint(1m, 1m - bowInset);
            var lowerBowTaper = new DeckContourPoint(bowTaperStart, 1m);

            points.AddRange(BuildCurve(
                new DeckContourPoint(bowTaperStart, 0m),
                upperBowFront,
                new DeckContourPoint(1m, 0m),
                settings.BowRoundnessPercentage,
                BowCurveSegments));
            points.Add(lowerBowFront);
            points.AddRange(BuildCurve(
                lowerBowFront,
                lowerBowTaper,
                new DeckContourPoint(1m, 1m),
                settings.BowRoundnessPercentage,
                BowCurveSegments));
        }

        if (settings.Shape == DeckShapeKind.NarrowedBowAndStern && settings.SternRoundnessPercentage > 0m)
        {
            var lowerSternTaper = new DeckContourPoint(sternTaperEnd, 1m);

            points.Add(lowerSternTaper);
            points.AddRange(BuildCurve(
                lowerSternTaper,
                new DeckContourPoint(0m, 1m - sternInset),
                new DeckContourPoint(0m, 1m),
                settings.SternRoundnessPercentage,
                SternCurveSegments));
        }
        else
        {
            points.Add(new(sternTaperEnd, 1m));
            points.Add(new(0m, 1m - sternInset));
        }

        return RemoveConsecutiveDuplicates(points);
    }

    private static void ValidatePercentage(decimal percentage, string parameterName)
    {
        if (percentage is < 10m or > 100m)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Deck contour width percentages must be between 10 and 100.");
        }
    }

    private static void ValidateTaperPercentage(decimal percentage, string parameterName)
    {
        if (percentage is < 0m or > 50m)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
            "Deck contour taper percentages must be between 0 and 50.");
        }
    }

    private static void ValidateRoundnessPercentage(decimal percentage, string parameterName)
    {
        if (percentage is < 0m or > 100m)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Deck contour roundness percentages must be between 0 and 100.");
        }
    }

    private static IReadOnlyList<DeckContourPoint> BuildCurve(
        DeckContourPoint start,
        DeckContourPoint end,
        DeckContourPoint roundedControl,
        decimal roundnessPercentage,
        int segments)
    {
        var roundness = roundnessPercentage / 100m;
        var straightControl = new DeckContourPoint(
            (start.XRatio + end.XRatio) / 2m,
            (start.YRatio + end.YRatio) / 2m);
        var control = new DeckContourPoint(
            Interpolate(straightControl.XRatio, roundedControl.XRatio, roundness),
            Interpolate(straightControl.YRatio, roundedControl.YRatio, roundness));
        var points = new List<DeckContourPoint>(segments);

        for (var segment = 1; segment <= segments; segment++)
        {
            var t = (decimal)segment / segments;
            points.Add(CalculateQuadraticBezierPoint(start, control, end, t));
        }

        return points;
    }

    private static DeckContourPoint CalculateQuadraticBezierPoint(
        DeckContourPoint start,
        DeckContourPoint control,
        DeckContourPoint end,
        decimal t)
    {
        var inverseT = 1m - t;
        var startWeight = inverseT * inverseT;
        var controlWeight = 2m * inverseT * t;
        var endWeight = t * t;

        return new DeckContourPoint(
            (startWeight * start.XRatio) + (controlWeight * control.XRatio) + (endWeight * end.XRatio),
            (startWeight * start.YRatio) + (controlWeight * control.YRatio) + (endWeight * end.YRatio));
    }

    private static decimal Interpolate(decimal start, decimal end, decimal amount)
    {
        return start + ((end - start) * amount);
    }

    private static IReadOnlyList<DeckContourPoint> RemoveConsecutiveDuplicates(IReadOnlyList<DeckContourPoint> points)
    {
        var cleaned = new List<DeckContourPoint>(points.Count);
        foreach (var point in points)
        {
            if (cleaned.Count == 0 || cleaned[^1] != point)
            {
                cleaned.Add(point);
            }
        }

        return cleaned;
    }
}
