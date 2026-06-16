using CommunityToolkit.Mvvm.ComponentModel;
using DeckPlanking.Core.Measurement;
using DeckPlanking.Core.Patterns;

namespace DeckPlanking.App.ViewModels;

public sealed class ScaleInputViewModel : ObservableObject
{
    private double realPlankLength = 9;
    private double decimalScale = 64;
    private double imperialInchesPerFoot = 1d / 6d;
    private OptionItem<LengthUnit> selectedLengthUnit;
    private OptionItem<ScaleMode> selectedScaleMode;
    private OptionItem<ShiftPatternKind> selectedPattern;
    private string rawScaleLengthText = string.Empty;
    private string cutLengthText = string.Empty;
    private string segmentLengthText = string.Empty;
    private string imperialDisplayText = string.Empty;
    private string validationMessage = string.Empty;
    private bool isDecimalScale = true;

    public ScaleInputViewModel()
    {
        LengthUnits =
        [
            new("Meters", LengthUnit.Meters),
            new("Feet", LengthUnit.Feet)
        ];

        ScaleModes =
        [
            new("Decimal scale 1:N", ScaleMode.Decimal),
            new("Imperial inches per foot", ScaleMode.ImperialInchesPerFoot)
        ];

        Patterns =
        [
            new("Every 2", ShiftPatternKind.Every2),
            new("Every 3", ShiftPatternKind.Every3),
            new("Every 4", ShiftPatternKind.Every4),
            new("Every 5", ShiftPatternKind.Every5)
        ];

        selectedLengthUnit = LengthUnits[0];
        selectedScaleMode = ScaleModes[0];
        selectedPattern = Patterns[3];

        Recalculate();
    }

    public IReadOnlyList<OptionItem<LengthUnit>> LengthUnits { get; }

    public IReadOnlyList<OptionItem<ScaleMode>> ScaleModes { get; }

    public IReadOnlyList<OptionItem<ShiftPatternKind>> Patterns { get; }

    public double RealPlankLength
    {
        get => realPlankLength;
        set => SetAndRecalculate(ref realPlankLength, value);
    }

    public double DecimalScale
    {
        get => decimalScale;
        set => SetAndRecalculate(ref decimalScale, value);
    }

    public double ImperialInchesPerFoot
    {
        get => imperialInchesPerFoot;
        set => SetAndRecalculate(ref imperialInchesPerFoot, value);
    }

    public OptionItem<LengthUnit> SelectedLengthUnit
    {
        get => selectedLengthUnit;
        set => SetAndRecalculate(ref selectedLengthUnit, value);
    }

    public OptionItem<ScaleMode> SelectedScaleMode
    {
        get => selectedScaleMode;
        set
        {
            if (SetProperty(ref selectedScaleMode, value))
            {
                IsDecimalScale = selectedScaleMode.Value == ScaleMode.Decimal;
                Recalculate();
            }
        }
    }

    public OptionItem<ShiftPatternKind> SelectedPattern
    {
        get => selectedPattern;
        set => SetAndRecalculate(ref selectedPattern, value);
    }

    public string RawScaleLengthText
    {
        get => rawScaleLengthText;
        private set => SetProperty(ref rawScaleLengthText, value);
    }

    public string CutLengthText
    {
        get => cutLengthText;
        private set => SetProperty(ref cutLengthText, value);
    }

    public string SegmentLengthText
    {
        get => segmentLengthText;
        private set => SetProperty(ref segmentLengthText, value);
    }

    public string ImperialDisplayText
    {
        get => imperialDisplayText;
        private set => SetProperty(ref imperialDisplayText, value);
    }

    public string ValidationMessage
    {
        get => validationMessage;
        private set => SetProperty(ref validationMessage, value);
    }

    public bool IsDecimalScale
    {
        get => isDecimalScale;
        private set => SetProperty(ref isDecimalScale, value);
    }

    public bool IsImperialScale => !IsDecimalScale;

    private void SetAndRecalculate<T>(ref T field, T value)
    {
        if (SetProperty(ref field, value))
        {
            Recalculate();
        }
    }

    private void Recalculate()
    {
        OnPropertyChanged(nameof(IsImperialScale));

        try
        {
            var scaleSettings = SelectedScaleMode.Value == ScaleMode.Decimal
                ? ScaleSettings.DecimalScale((decimal)DecimalScale)
                : ScaleSettings.ImperialInchesPerFoot((decimal)ImperialInchesPerFoot);

            var result = PlankLengthCalculator.Calculate(new PlankLengthRequest(
                (decimal)RealPlankLength,
                SelectedLengthUnit.Value,
                scaleSettings,
                SelectedPattern.Value,
                MetricDecimalPlaces: 1,
                ImperialFractionDenominator: 16));

            RawScaleLengthText = $"{result.RawScaleLengthMillimeters:0.###} mm";
            CutLengthText = $"{result.CutLengthMillimeters:0.#} mm";
            SegmentLengthText = $"{result.SegmentLengthMillimeters:0.###} mm";
            ImperialDisplayText = $"{result.DisplayLengthInches:0.####} in";
            ValidationMessage = string.Empty;
        }
        catch (ArgumentOutOfRangeException)
        {
            RawScaleLengthText = "-";
            CutLengthText = "-";
            SegmentLengthText = "-";
            ImperialDisplayText = "-";
            ValidationMessage = "Enter positive values for length and scale.";
        }
    }
}
