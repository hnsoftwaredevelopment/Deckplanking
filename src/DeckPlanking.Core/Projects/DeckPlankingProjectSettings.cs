using DeckPlanking.Core.Configuration;
using DeckPlanking.Core.Measurement;
using DeckPlanking.Core.Patterns;
using DeckPlanking.Core.Preview;

namespace DeckPlanking.Core.Projects;

public sealed record DeckPlankingProjectSettings(
    double RealPlankLength,
    LengthUnit LengthUnit,
    ScaleMode ScaleMode,
    double DecimalScale,
    double ImperialInchesPerFoot,
    double DeckLengthMillimeters,
    DeckShapeKind DeckShape,
    double BowWidthPercentage,
    double SternWidthPercentage,
    double BowTaperLengthPercentage,
    double SternTaperLengthPercentage,
    double DeckWidthMillimeters,
    double PlankWidthMillimeters,
    double KingPlankWidthMillimeters,
    RowInputMode RowInputMode,
    int RowCount,
    int StartPoint,
    ShiftPatternKind ShiftPattern,
    bool UseKingPlank,
    DeckOrientation DeckOrientation,
    TrenailPatternKind TrenailPattern);
