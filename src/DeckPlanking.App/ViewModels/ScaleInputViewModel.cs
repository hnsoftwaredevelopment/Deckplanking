using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeckPlanking.Core.Measurement;
using DeckPlanking.Core.Patterns;
using DeckPlanking.Core.Preview;
using System.Collections.ObjectModel;

namespace DeckPlanking.App.ViewModels;

public sealed class ScaleInputViewModel : ObservableObject
{
    private double realPlankLength = 9;
    private double decimalScale = 64;
    private double deckLengthMillimeters = 600;
    private double imperialInchesPerFoot = 1d / 6d;
    private int rowCount = 8;
    private int startPoint;
    private OptionItem<LengthUnit> selectedLengthUnit;
    private OptionItem<ScaleMode> selectedScaleMode;
    private OptionItem<ShiftPatternKind> selectedPattern;
    private string rawScaleLengthText = string.Empty;
    private string cutLengthText = string.Empty;
    private string segmentLengthText = string.Empty;
    private string imperialDisplayText = string.Empty;
    private string validationMessage = string.Empty;
    private decimal segmentLengthMillimeters;
    private bool isSeamTableVisible;
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

        ToggleSeamTableCommand = new RelayCommand(ToggleSeamTable);
        Recalculate();
    }

    public IReadOnlyList<OptionItem<LengthUnit>> LengthUnits { get; }

    public IReadOnlyList<OptionItem<ScaleMode>> ScaleModes { get; }

    public IReadOnlyList<OptionItem<ShiftPatternKind>> Patterns { get; }

    public ObservableCollection<PatternPreviewRow> PatternRows { get; } = [];

    public IRelayCommand ToggleSeamTableCommand { get; }

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

    public double DeckLengthMillimeters
    {
        get => deckLengthMillimeters;
        set => SetAndRecalculate(ref deckLengthMillimeters, value);
    }

    public double ImperialInchesPerFoot
    {
        get => imperialInchesPerFoot;
        set => SetAndRecalculate(ref imperialInchesPerFoot, value);
    }

    public int RowCount
    {
        get => rowCount;
        set => SetAndRecalculate(ref rowCount, value);
    }

    public int StartPoint
    {
        get => startPoint;
        set => SetAndRecalculate(ref startPoint, value);
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

    public decimal SegmentLengthMillimeters
    {
        get => segmentLengthMillimeters;
        private set => SetProperty(ref segmentLengthMillimeters, value);
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

    public bool IsSeamTableVisible
    {
        get => isSeamTableVisible;
        private set
        {
            if (SetProperty(ref isSeamTableVisible, value))
            {
                OnPropertyChanged(nameof(SeamTableToggleText));
            }
        }
    }

    public string SeamTableToggleText => IsSeamTableVisible ? "Hide seam details" : "Show seam details";

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
            SegmentLengthMillimeters = result.SegmentLengthMillimeters;
            ImperialDisplayText = $"{result.DisplayLengthInches:0.####} in";
            UpdatePatternRows(result.CutLengthMillimeters);
            ValidationMessage = string.Empty;
        }
        catch (ArgumentOutOfRangeException)
        {
            RawScaleLengthText = "-";
            CutLengthText = "-";
            SegmentLengthText = "-";
            SegmentLengthMillimeters = 0;
            ImperialDisplayText = "-";
            PatternRows.Clear();
            ValidationMessage = "Enter positive values for length and scale.";
        }
    }

    private void ToggleSeamTable()
    {
        IsSeamTableVisible = !IsSeamTableVisible;
    }

    private void UpdatePatternRows(decimal plankLengthMillimeters)
    {
        PatternRows.Clear();

        var rows = PatternPreviewBuilder.Build(
            plankLengthMillimeters,
            (decimal)DeckLengthMillimeters,
            SelectedPattern.Value,
            RowCount,
            StartPoint);

        foreach (var row in rows)
        {
            PatternRows.Add(row);
        }
    }
}
