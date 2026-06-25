using DeckPlanking.Core.Configuration;
using DeckPlanking.Core.Measurement;
using DeckPlanking.Core.Patterns;
using DeckPlanking.Core.Preview;
using DeckPlanking.Core.Projects;

namespace DeckPlanking.Core.Tests;

public sealed class ProjectJsonSerializerTests
{
    [Fact]
    public void CreatesVersionOneProjectDocument()
    {
        var settings = CreateRepresentativeSettings();
        var savedAt = new DateTimeOffset(2026, 6, 19, 16, 45, 0, TimeSpan.Zero);

        var document = DeckPlankingProjectDocument.Create(settings, savedAt);

        Assert.Equal(1, document.SchemaVersion);
        Assert.Equal(savedAt, document.SavedAt);
        Assert.Equal(settings, document.Settings);
    }

    [Fact]
    public void RoundTripsRepresentativeProject()
    {
        var project = new DeckPlankingProjectDocument(
            1,
            new DateTimeOffset(2026, 6, 19, 10, 30, 0, TimeSpan.Zero),
            CreateRepresentativeSettings());

        var json = ProjectJsonSerializer.Serialize(project);
        var restored = ProjectJsonSerializer.Deserialize(json);

        Assert.Equal(project, restored);
    }

    [Fact]
    public void SerializesEnumsAsStrings()
    {
        var project = new DeckPlankingProjectDocument(
            1,
            new DateTimeOffset(2026, 6, 19, 10, 30, 0, TimeSpan.Zero),
            new DeckPlankingProjectSettings(
                RealPlankLength: 12,
                LengthUnit: LengthUnit.Feet,
                ScaleMode: ScaleMode.ImperialInchesPerFoot,
                DecimalScale: 48,
                ImperialInchesPerFoot: 0.25,
                DeckLengthMillimeters: 480,
                DeckShape: DeckShapeKind.NarrowedBowAndStern,
                BowWidthPercentage: 60,
                SternWidthPercentage: 90,
                BowTaperLengthPercentage: 30,
                SternTaperLengthPercentage: 10,
                BowRoundnessPercentage: 75,
                SternRoundnessPercentage: 25,
                DeckWidthMillimeters: 72,
                PlankWidthMillimeters: 4,
                KingPlankWidthMillimeters: 7,
                RowInputMode: RowInputMode.FromDeckWidth,
                RowCount: 6,
                StartPoint: 1,
                ShiftPattern: ShiftPatternKind.Every3,
                UseKingPlank: false,
                DeckOrientation: DeckOrientation.BowLeft,
                TrenailPattern: TrenailPatternKind.TwoPerPlankEnd));

        var json = ProjectJsonSerializer.Serialize(project);

        Assert.Contains("\"lengthUnit\": \"Feet\"", json);
        Assert.Contains("\"shiftPattern\": \"Every3\"", json);
        Assert.Contains("\"rowInputMode\": \"FromDeckWidth\"", json);
        Assert.Contains("\"kingPlankWidthMillimeters\": 7", json);
        Assert.Contains("\"deckShape\": \"NarrowedBowAndStern\"", json);
        Assert.Contains("\"bowWidthPercentage\": 60", json);
        Assert.Contains("\"bowTaperLengthPercentage\": 30", json);
        Assert.Contains("\"bowRoundnessPercentage\": 75", json);
        Assert.Contains("\"sternRoundnessPercentage\": 25", json);
    }

    [Fact]
    public void RejectsUnsupportedFutureSchemaVersion()
    {
        const string json = """
            {
              "schemaVersion": 2,
              "savedAt": "2026-06-19T10:30:00+00:00",
              "settings": {
                "realPlankLength": 9,
                "lengthUnit": "Meters",
                "scaleMode": "Decimal",
                "decimalScale": 64,
                "imperialInchesPerFoot": 0.1666666667,
                "deckLengthMillimeters": 600,
                "rowCount": 8,
                "startPoint": 0,
                "shiftPattern": "Every5",
                "useKingPlank": false,
                "deckOrientation": "BowLeft",
                "trenailPattern": "TwoPerPlankEnd"
              }
            }
            """;

        var exception = Assert.Throws<InvalidDataException>(
            () => ProjectJsonSerializer.Deserialize(json));

        Assert.Equal("Project schema version 2 is not supported.", exception.Message);
    }

    private static DeckPlankingProjectSettings CreateRepresentativeSettings()
    {
        return new DeckPlankingProjectSettings(
            RealPlankLength: 9,
            LengthUnit: LengthUnit.Meters,
            ScaleMode: ScaleMode.Decimal,
            DecimalScale: 64,
            ImperialInchesPerFoot: 1d / 6d,
            DeckLengthMillimeters: 600,
            DeckShape: DeckShapeKind.NarrowedBow,
            BowWidthPercentage: 65,
            SternWidthPercentage: 100,
            BowTaperLengthPercentage: 25,
            SternTaperLengthPercentage: 10,
            BowRoundnessPercentage: 50,
            SternRoundnessPercentage: 15,
            DeckWidthMillimeters: 85,
            PlankWidthMillimeters: 5,
            KingPlankWidthMillimeters: 10,
            RowInputMode: RowInputMode.FromDeckWidth,
            RowCount: 8,
            StartPoint: 2,
            ShiftPattern: ShiftPatternKind.Every5,
            UseKingPlank: true,
            DeckOrientation: DeckOrientation.SternLeft,
            TrenailPattern: TrenailPatternKind.OneAlternating);
    }
}
