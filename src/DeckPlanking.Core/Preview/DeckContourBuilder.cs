using DeckPlanking.Core.Configuration;

namespace DeckPlanking.Core.Preview;

public static class DeckContourBuilder
{
    public static IReadOnlyList<DeckContourPoint> Build(DeckContourSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        ValidatePercentage(settings.BowWidthPercentage, nameof(settings.BowWidthPercentage));
        ValidatePercentage(settings.SternWidthPercentage, nameof(settings.SternWidthPercentage));
        ValidateTaperPercentage(settings.BowTaperLengthPercentage, nameof(settings.BowTaperLengthPercentage));
        ValidateTaperPercentage(settings.SternTaperLengthPercentage, nameof(settings.SternTaperLengthPercentage));

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

        DeckContourPoint[] points =
        [
            new(0m, sternInset),
            new(sternTaperEnd, 0m),
            new(bowTaperStart, 0m),
            new(1m, bowInset),
            new(1m, 1m - bowInset),
            new(bowTaperStart, 1m),
            new(sternTaperEnd, 1m),
            new(0m, 1m - sternInset)
        ];

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
