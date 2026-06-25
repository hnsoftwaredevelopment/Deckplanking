using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeckPlanking.App.Localization;
using DeckPlanking.App.Settings;
using DeckPlanking.Core.Configuration;
using DeckPlanking.Core.Measurement;
using DeckPlanking.Core.Patterns;
using DeckPlanking.Core.Preview;
using DeckPlanking.Core.Projects;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DeckPlanking.App.ViewModels;

public sealed class ScaleInputViewModel : ObservableObject
{
    private const double FeetPerMeter = 3.280839895013123d;
    private double realPlankLength = 9;
    private double decimalScale = 64;
    private double deckLengthMillimeters = 600;
    private double bowWidthPercentage = 60;
    private double sternWidthPercentage = 95;
    private double bowTaperLengthPercentage = 25;
    private double sternTaperLengthPercentage = 10;
    private double bowRoundnessPercentage;
    private double sternRoundnessPercentage;
    private double deckWidthMillimeters = 80;
    private double plankWidthMillimeters = 5;
    private double kingPlankWidthMillimeters = 5;
    private double imperialInchesPerFoot = 1d / 6d;
    private int rowCount = 8;
    private int startPoint;
    private OptionItem<LengthUnit> selectedLengthUnit;
    private OptionItem<ScaleMode> selectedScaleMode;
    private OptionItem<RowInputMode> selectedRowInputMode;
    private OptionItem<DeckShapeKind> selectedDeckShape;
    private OptionItem<ShiftPatternKind> selectedPattern;
    private OptionItem<DeckOrientation> selectedDeckOrientation;
    private OptionItem<TrenailPatternKind> selectedTrenailPattern;
    private string rawScaleLengthText = string.Empty;
    private string cutLengthText = string.Empty;
    private string segmentLengthText = string.Empty;
    private string imperialDisplayText = string.Empty;
    private string validationMessage = string.Empty;
    private decimal cutLengthMillimeters;
    private decimal segmentLengthMillimeters;
    private bool isSeamTableVisible;
    private bool useKingPlank;
    private bool isDecimalScale = true;
    private bool isApplyingProjectSettings;

    public ScaleInputViewModel()
    {
        RefreshLocalizedOptions();

        TrenailPatterns =
        [
            new(T("NoTrenails"), TrenailPatternKind.None),
            new("oo|oo", TrenailPatternKind.TwoPerPlankEnd),
            new("o|o", TrenailPatternKind.OneCentered),
            new("^|v", TrenailPatternKind.OneAlternating)
        ];

        selectedLengthUnit = LengthUnits[0];
        selectedScaleMode = ScaleModes[0];
        selectedRowInputMode = RowInputModes[0];
        selectedDeckShape = DeckShapes[0];
        selectedPattern = Patterns[3];
        selectedDeckOrientation = DeckOrientations[0];
        selectedTrenailPattern = TrenailPatterns[1];

        ToggleSeamTableCommand = new RelayCommand(ToggleSeamTable);
        SelectTrenailPatternCommand = new RelayCommand<TrenailPatternKind>(SelectTrenailPattern);
        LocalizationResourceManager.Instance.PropertyChanged += OnLocalizationChanged;
        AppPreferencesStore.PreferenceChanged += OnAppPreferenceChanged;
        Recalculate();
    }

    public IReadOnlyList<OptionItem<LengthUnit>> LengthUnits { get; private set; } = [];

    public IReadOnlyList<OptionItem<ScaleMode>> ScaleModes { get; private set; } = [];

    public IReadOnlyList<OptionItem<RowInputMode>> RowInputModes { get; private set; } = [];

    public IReadOnlyList<OptionItem<DeckShapeKind>> DeckShapes { get; private set; } = [];

    public IReadOnlyList<OptionItem<ShiftPatternKind>> Patterns { get; private set; } = [];

    public IReadOnlyList<OptionItem<DeckOrientation>> DeckOrientations { get; private set; } = [];

    public IReadOnlyList<OptionItem<TrenailPatternKind>> TrenailPatterns { get; }

    public ObservableCollection<PatternPreviewRow> PatternRows { get; } = [];

    public IRelayCommand ToggleSeamTableCommand { get; }

    public IRelayCommand<TrenailPatternKind> SelectTrenailPatternCommand { get; }

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
        set
        {
            if (SetAndRecalculateProperty(ref deckLengthMillimeters, value))
            {
                OnPropertyChanged(nameof(DeckLengthInput));
            }
        }
    }

    public double DeckLengthInput
    {
        get => DisplayLengthFormatter.ToInputValue(DeckLengthMillimeters);
        set => DeckLengthMillimeters = DisplayLengthFormatter.FromInputValue(value);
    }

    public OptionItem<DeckShapeKind> SelectedDeckShape
    {
        get => selectedDeckShape;
        set
        {
            if (SetAndRecalculateProperty(ref selectedDeckShape, value))
            {
                OnPropertyChanged(nameof(IsBowWidthPercentageVisible));
                OnPropertyChanged(nameof(IsSternWidthPercentageVisible));
            }
        }
    }

    public double BowWidthPercentage
    {
        get => bowWidthPercentage;
        set => SetAndRecalculate(ref bowWidthPercentage, value);
    }

    public double SternWidthPercentage
    {
        get => sternWidthPercentage;
        set => SetAndRecalculate(ref sternWidthPercentage, value);
    }

    public double BowTaperLengthPercentage
    {
        get => bowTaperLengthPercentage;
        set => SetAndRecalculate(ref bowTaperLengthPercentage, value);
    }

    public double SternTaperLengthPercentage
    {
        get => sternTaperLengthPercentage;
        set => SetAndRecalculate(ref sternTaperLengthPercentage, value);
    }

    public double BowRoundnessPercentage
    {
        get => bowRoundnessPercentage;
        set => SetAndRecalculate(ref bowRoundnessPercentage, value);
    }

    public double SternRoundnessPercentage
    {
        get => sternRoundnessPercentage;
        set => SetAndRecalculate(ref sternRoundnessPercentage, value);
    }

    public double DeckWidthMillimeters
    {
        get => deckWidthMillimeters;
        set
        {
            if (SetAndRecalculateProperty(ref deckWidthMillimeters, value))
            {
                OnPropertyChanged(nameof(DeckWidthInput));
            }
        }
    }

    public double DeckWidthInput
    {
        get => DisplayLengthFormatter.ToInputValue(DeckWidthMillimeters);
        set => DeckWidthMillimeters = DisplayLengthFormatter.FromInputValue(value);
    }

    public double PlankWidthMillimeters
    {
        get => plankWidthMillimeters;
        set
        {
            if (SetAndRecalculateProperty(ref plankWidthMillimeters, value))
            {
                OnPropertyChanged(nameof(PlankWidthInput));
            }
        }
    }

    public double PlankWidthInput
    {
        get => DisplayLengthFormatter.ToInputValue(PlankWidthMillimeters);
        set => PlankWidthMillimeters = DisplayLengthFormatter.FromInputValue(value);
    }

    public double KingPlankWidthMillimeters
    {
        get => kingPlankWidthMillimeters;
        set
        {
            if (SetAndRecalculateProperty(ref kingPlankWidthMillimeters, value))
            {
                OnPropertyChanged(nameof(KingPlankWidthInput));
            }
        }
    }

    public double KingPlankWidthInput
    {
        get => DisplayLengthFormatter.ToInputValue(KingPlankWidthMillimeters);
        set => KingPlankWidthMillimeters = DisplayLengthFormatter.FromInputValue(value);
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

    public bool UseKingPlank
    {
        get => useKingPlank;
        set
        {
            if (!SetProperty(ref useKingPlank, value))
            {
                return;
            }

            OnPropertyChanged(nameof(StartPointHelpText));

            if (SelectedRowInputMode.Value == RowInputMode.FromDeckWidth)
            {
                OnPropertyChanged(nameof(IsKingPlankWidthVisible));
                if (!isApplyingProjectSettings)
                {
                    Recalculate();
                }
            }
        }
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
                if (!isApplyingProjectSettings)
                {
                    Recalculate();
                }
            }
        }
    }

    public OptionItem<RowInputMode> SelectedRowInputMode
    {
        get => selectedRowInputMode;
        set
        {
            if (SetAndRecalculateProperty(ref selectedRowInputMode, value))
            {
                OnPropertyChanged(nameof(IsManualRowInput));
                OnPropertyChanged(nameof(IsWidthBasedRowInput));
                OnPropertyChanged(nameof(IsKingPlankWidthVisible));
            }
        }
    }

    public OptionItem<ShiftPatternKind> SelectedPattern
    {
        get => selectedPattern;
        set => SetAndRecalculate(ref selectedPattern, value);
    }

    public OptionItem<DeckOrientation> SelectedDeckOrientation
    {
        get => selectedDeckOrientation;
        set => SetProperty(ref selectedDeckOrientation, value);
    }

    public OptionItem<TrenailPatternKind> SelectedTrenailPattern
    {
        get => selectedTrenailPattern;
        set
        {
            if (SetProperty(ref selectedTrenailPattern, value))
            {
                OnPropertyChanged(nameof(SelectedTrenailPatternIcon));
                OnPropertyChanged(nameof(SelectedTrenailPatternDescription));
                OnPropertyChanged(nameof(IsNoTrenailsSelected));
                OnPropertyChanged(nameof(IsTwoTrenailsSelected));
                OnPropertyChanged(nameof(IsOneCenteredTrenailSelected));
                OnPropertyChanged(nameof(IsOneAlternatingTrenailSelected));
                OnPropertyChanged(nameof(NoTrenailsOpacity));
                OnPropertyChanged(nameof(TwoTrenailsOpacity));
                OnPropertyChanged(nameof(OneCenteredTrenailOpacity));
                OnPropertyChanged(nameof(OneAlternatingTrenailOpacity));
            }
        }
    }

    public string SelectedTrenailPatternIcon => SelectedTrenailPattern.Value switch
    {
        TrenailPatternKind.None => "trenail_none.png",
        TrenailPatternKind.TwoPerPlankEnd => "trenail_two.png",
        TrenailPatternKind.OneCentered => "trenail_center.png",
        TrenailPatternKind.OneAlternating => "trenail_alternating.png",
        _ => "trenail_two.png"
    };

    public string SelectedTrenailPatternDescription => SelectedTrenailPattern.Value switch
    {
        TrenailPatternKind.None => T("NoTrenails"),
        TrenailPatternKind.TwoPerPlankEnd => T("TwoTrenailsPerPlankEnd"),
        TrenailPatternKind.OneCentered => T("OneCenteredTrenail"),
        TrenailPatternKind.OneAlternating => T("OneAlternatingTrenail"),
        _ => T("TwoTrenailsPerPlankEnd")
    };

    public bool IsNoTrenailsSelected => SelectedTrenailPattern.Value == TrenailPatternKind.None;

    public bool IsTwoTrenailsSelected => SelectedTrenailPattern.Value == TrenailPatternKind.TwoPerPlankEnd;

    public bool IsOneCenteredTrenailSelected => SelectedTrenailPattern.Value == TrenailPatternKind.OneCentered;

    public bool IsOneAlternatingTrenailSelected => SelectedTrenailPattern.Value == TrenailPatternKind.OneAlternating;

    public double NoTrenailsOpacity => IsNoTrenailsSelected ? 1 : 0.45;

    public double TwoTrenailsOpacity => IsTwoTrenailsSelected ? 1 : 0.45;

    public double OneCenteredTrenailOpacity => IsOneCenteredTrenailSelected ? 1 : 0.45;

    public double OneAlternatingTrenailOpacity => IsOneAlternatingTrenailSelected ? 1 : 0.45;

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

    public decimal CutLengthMillimeters
    {
        get => cutLengthMillimeters;
        private set => SetProperty(ref cutLengthMillimeters, value);
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

    public bool IsManualRowInput => SelectedRowInputMode.Value == RowInputMode.Manual;

    public bool IsWidthBasedRowInput => SelectedRowInputMode.Value == RowInputMode.FromDeckWidth;

    public bool IsKingPlankWidthVisible => IsWidthBasedRowInput && UseKingPlank;

    public string StartPointHelpText => UseKingPlank
        ? T("StartPointRelativeToKingPlank")
        : T("StartPointRelativeToCenterlineRow");

    public bool IsBowWidthPercentageVisible => SelectedDeckShape.Value != DeckShapeKind.Rectangular;

    public bool IsSternWidthPercentageVisible => SelectedDeckShape.Value == DeckShapeKind.NarrowedBowAndStern;

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

    public string SeamTableToggleText => IsSeamTableVisible ? T("HideSeamDetails") : T("ShowSeamDetails");

    public string DimensionInputUnitText => DisplayLengthFormatter.InputUnitText;

    public string SeamPositionsHeaderText => $"{T("Seams")} ({DisplayLengthFormatter.InputUnitText})";

    public DeckPlankingProjectSettings CaptureProjectSettings()
    {
        return new DeckPlankingProjectSettings(
            RealPlankLength,
            SelectedLengthUnit.Value,
            SelectedScaleMode.Value,
            DecimalScale,
            ImperialInchesPerFoot,
            DeckLengthMillimeters,
            SelectedDeckShape.Value,
            BowWidthPercentage,
            SternWidthPercentage,
            BowTaperLengthPercentage,
            SternTaperLengthPercentage,
            BowRoundnessPercentage,
            SternRoundnessPercentage,
            DeckWidthMillimeters,
            PlankWidthMillimeters,
            KingPlankWidthMillimeters,
            SelectedRowInputMode.Value,
            RowCount,
            StartPoint,
            SelectedPattern.Value,
            UseKingPlank,
            SelectedDeckOrientation.Value,
            SelectedTrenailPattern.Value);
    }

    public void ApplyProjectSettings(DeckPlankingProjectSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        isApplyingProjectSettings = true;
        try
        {
            RealPlankLength = settings.RealPlankLength;
            SelectedLengthUnit = FindOption(LengthUnits, settings.LengthUnit);
            SelectedScaleMode = FindOption(ScaleModes, settings.ScaleMode);
            DecimalScale = settings.DecimalScale;
            ImperialInchesPerFoot = settings.ImperialInchesPerFoot;
            DeckLengthMillimeters = settings.DeckLengthMillimeters;
            SelectedDeckShape = FindOption(DeckShapes, settings.DeckShape);
            BowWidthPercentage = settings.BowWidthPercentage > 0 ? settings.BowWidthPercentage : 100;
            SternWidthPercentage = settings.SternWidthPercentage > 0 ? settings.SternWidthPercentage : 100;
            BowTaperLengthPercentage = settings.BowTaperLengthPercentage > 0 ? settings.BowTaperLengthPercentage : 25;
            SternTaperLengthPercentage = settings.SternTaperLengthPercentage > 0 ? settings.SternTaperLengthPercentage : 10;
            BowRoundnessPercentage = settings.BowRoundnessPercentage >= 0 ? settings.BowRoundnessPercentage : 0;
            SternRoundnessPercentage = settings.SternRoundnessPercentage >= 0 ? settings.SternRoundnessPercentage : 0;
            DeckWidthMillimeters = settings.DeckWidthMillimeters > 0 ? settings.DeckWidthMillimeters : DeckWidthMillimeters;
            PlankWidthMillimeters = settings.PlankWidthMillimeters > 0 ? settings.PlankWidthMillimeters : PlankWidthMillimeters;
            KingPlankWidthMillimeters = settings.KingPlankWidthMillimeters > 0
                ? settings.KingPlankWidthMillimeters
                : PlankWidthMillimeters;
            SelectedRowInputMode = FindOption(RowInputModes, settings.RowInputMode);
            RowCount = settings.RowCount;
            StartPoint = settings.StartPoint;
            SelectedPattern = FindOption(Patterns, settings.ShiftPattern);
            UseKingPlank = settings.UseKingPlank;
            SelectedDeckOrientation = FindOption(DeckOrientations, settings.DeckOrientation);
            SelectedTrenailPattern = FindOption(TrenailPatterns, settings.TrenailPattern);
        }
        finally
        {
            isApplyingProjectSettings = false;
        }

        Recalculate();
        NotifyProjectInputBindingsChanged();
    }

    private void SetAndRecalculate<T>(ref T field, T value)
    {
        SetAndRecalculateProperty(ref field, value);
    }

    private bool SetAndRecalculateProperty<T>(ref T field, T value)
    {
        if (SetProperty(ref field, value))
        {
            if (!isApplyingProjectSettings)
            {
                Recalculate();
            }
            return true;
        }

        return false;
    }

    private void Recalculate()
    {
        OnPropertyChanged(nameof(IsImperialScale));

        try
        {
            ValidateDeckContour();

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

            RawScaleLengthText = DisplayLengthFormatter.Format(result.RawScaleLengthMillimeters);
            CutLengthText = DisplayLengthFormatter.Format(result.CutLengthMillimeters);
            CutLengthMillimeters = result.CutLengthMillimeters;
            SegmentLengthText = DisplayLengthFormatter.Format(result.SegmentLengthMillimeters);
            SegmentLengthMillimeters = result.SegmentLengthMillimeters;
            ImperialDisplayText = LengthDisplayFormatter.FormatInchesFromMillimeters(result.CutLengthMillimeters, 16);
            UpdateCalculatedRowCount();
            UpdatePatternRows(result.CutLengthMillimeters);
            ValidationMessage = string.Empty;
        }
        catch (ArgumentOutOfRangeException)
        {
            RawScaleLengthText = "-";
            CutLengthText = "-";
            CutLengthMillimeters = 0;
            SegmentLengthText = "-";
            SegmentLengthMillimeters = 0;
            ImperialDisplayText = "-";
            PatternRows.Clear();
            ValidationMessage = T("ValidationPositiveValues");
        }
    }

    private void ToggleSeamTable()
    {
        IsSeamTableVisible = !IsSeamTableVisible;
    }

    private void SelectTrenailPattern(TrenailPatternKind trenailPatternKind)
    {
        SelectedTrenailPattern = TrenailPatterns.First(pattern => pattern.Value == trenailPatternKind);
    }

    private void OnLocalizationChanged(object? sender, PropertyChangedEventArgs e)
    {
        var lengthUnit = SelectedLengthUnit.Value;
        var scaleMode = SelectedScaleMode.Value;
        var rowInputMode = SelectedRowInputMode.Value;
        var deckShape = SelectedDeckShape.Value;
        var pattern = SelectedPattern.Value;
        var orientation = SelectedDeckOrientation.Value;

        RefreshLocalizedOptions();

        selectedLengthUnit = FindOption(LengthUnits, lengthUnit);
        selectedScaleMode = FindOption(ScaleModes, scaleMode);
        selectedRowInputMode = FindOption(RowInputModes, rowInputMode);
        selectedDeckShape = FindOption(DeckShapes, deckShape);
        selectedPattern = FindOption(Patterns, pattern);
        selectedDeckOrientation = FindOption(DeckOrientations, orientation);

        OnPropertyChanged(nameof(LengthUnits));
        OnPropertyChanged(nameof(ScaleModes));
        OnPropertyChanged(nameof(RowInputModes));
        OnPropertyChanged(nameof(DeckShapes));
        OnPropertyChanged(nameof(Patterns));
        OnPropertyChanged(nameof(DeckOrientations));
        OnPropertyChanged(nameof(SelectedLengthUnit));
        OnPropertyChanged(nameof(SelectedScaleMode));
        OnPropertyChanged(nameof(SelectedRowInputMode));
        OnPropertyChanged(nameof(SelectedDeckShape));
        OnPropertyChanged(nameof(SelectedPattern));
        OnPropertyChanged(nameof(SelectedDeckOrientation));
        OnPropertyChanged(nameof(SeamTableToggleText));
        OnPropertyChanged(nameof(SelectedTrenailPatternDescription));
        OnPropertyChanged(nameof(StartPointHelpText));
        OnPropertyChanged(nameof(IsBowWidthPercentageVisible));
        OnPropertyChanged(nameof(IsSternWidthPercentageVisible));

        if (!string.IsNullOrWhiteSpace(ValidationMessage))
        {
            ValidationMessage = T("ValidationPositiveValues");
        }
    }

    private void OnAppPreferenceChanged(object? sender, AppPreferencesChangedEventArgs e)
    {
        if (e.PreferenceName == AppPreferencesStore.DisplayUnitSystemPreferenceName)
        {
            ApplyDisplayUnitToFullSizeLength();
            NotifyDimensionInputsChanged();
            Recalculate();
        }
    }

    private void ApplyDisplayUnitToFullSizeLength()
    {
        var targetLengthUnit = AppPreferencesStore.GetDisplayUnitSystem() == DisplayUnitSystemOption.Imperial
            ? LengthUnit.Feet
            : LengthUnit.Meters;

        if (SelectedLengthUnit.Value == targetLengthUnit)
        {
            return;
        }

        realPlankLength = targetLengthUnit == LengthUnit.Feet
            ? Math.Round(realPlankLength * FeetPerMeter, 4, MidpointRounding.AwayFromZero)
            : Math.Round(realPlankLength / FeetPerMeter, 2, MidpointRounding.AwayFromZero);

        selectedLengthUnit = FindOption(LengthUnits, targetLengthUnit);
        OnPropertyChanged(nameof(RealPlankLength));
        OnPropertyChanged(nameof(SelectedLengthUnit));
    }

    private void NotifyDimensionInputsChanged()
    {
        OnPropertyChanged(nameof(DeckLengthInput));
        OnPropertyChanged(nameof(BowWidthPercentage));
        OnPropertyChanged(nameof(SternWidthPercentage));
        OnPropertyChanged(nameof(BowTaperLengthPercentage));
        OnPropertyChanged(nameof(SternTaperLengthPercentage));
        OnPropertyChanged(nameof(BowRoundnessPercentage));
        OnPropertyChanged(nameof(SternRoundnessPercentage));
        OnPropertyChanged(nameof(DeckWidthInput));
        OnPropertyChanged(nameof(PlankWidthInput));
        OnPropertyChanged(nameof(KingPlankWidthInput));
        OnPropertyChanged(nameof(DimensionInputUnitText));
        OnPropertyChanged(nameof(SeamPositionsHeaderText));
    }

    private void NotifyProjectInputBindingsChanged()
    {
        NotifyDimensionInputsChanged();
        OnPropertyChanged(nameof(RealPlankLength));
        OnPropertyChanged(nameof(DecimalScale));
        OnPropertyChanged(nameof(ImperialInchesPerFoot));
        OnPropertyChanged(nameof(RowCount));
        OnPropertyChanged(nameof(StartPoint));
        OnPropertyChanged(nameof(UseKingPlank));
        OnPropertyChanged(nameof(SelectedLengthUnit));
        OnPropertyChanged(nameof(SelectedScaleMode));
        OnPropertyChanged(nameof(SelectedRowInputMode));
        OnPropertyChanged(nameof(SelectedDeckShape));
        OnPropertyChanged(nameof(SelectedPattern));
        OnPropertyChanged(nameof(SelectedDeckOrientation));
        OnPropertyChanged(nameof(SelectedTrenailPattern));
        OnPropertyChanged(nameof(IsDecimalScale));
        OnPropertyChanged(nameof(IsImperialScale));
        OnPropertyChanged(nameof(IsManualRowInput));
        OnPropertyChanged(nameof(IsWidthBasedRowInput));
        OnPropertyChanged(nameof(IsKingPlankWidthVisible));
        OnPropertyChanged(nameof(IsBowWidthPercentageVisible));
        OnPropertyChanged(nameof(IsSternWidthPercentageVisible));
    }

    private void RefreshLocalizedOptions()
    {
        LengthUnits =
        [
            new(T("Meters"), LengthUnit.Meters),
            new(T("Feet"), LengthUnit.Feet)
        ];

        ScaleModes =
        [
            new(T("DecimalScaleMode"), ScaleMode.Decimal),
            new(T("ImperialScaleMode"), ScaleMode.ImperialInchesPerFoot)
        ];

        RowInputModes =
        [
            new(T("ManualRows"), RowInputMode.Manual),
            new(T("FromDeckWidth"), RowInputMode.FromDeckWidth)
        ];

        DeckShapes =
        [
            new(T("DeckShapeRectangular"), DeckShapeKind.Rectangular),
            new(T("DeckShapeNarrowedBow"), DeckShapeKind.NarrowedBow),
            new(T("DeckShapeNarrowedBowAndStern"), DeckShapeKind.NarrowedBowAndStern)
        ];

        Patterns =
        [
            new(T("PatternEvery2"), ShiftPatternKind.Every2),
            new(T("PatternEvery3"), ShiftPatternKind.Every3),
            new(T("PatternEvery4"), ShiftPatternKind.Every4),
            new(T("PatternEvery5"), ShiftPatternKind.Every5)
        ];

        DeckOrientations =
        [
            new(T("BowLeft"), DeckOrientation.BowLeft),
            new(T("SternLeft"), DeckOrientation.SternLeft)
        ];
    }

    private static string T(string key)
    {
        return LocalizationResourceManager.Instance[key];
    }

    private static OptionItem<T> FindOption<T>(IReadOnlyList<OptionItem<T>> options, T value)
        where T : notnull
    {
        return options.First(option => EqualityComparer<T>.Default.Equals(option.Value, value));
    }

    private void UpdateCalculatedRowCount()
    {
        if (SelectedRowInputMode.Value != RowInputMode.FromDeckWidth)
        {
            return;
        }

        var calculatedRows = DeckRowCountCalculator.CalculateRowsPerSide(
            (decimal)DeckWidthMillimeters,
            (decimal)PlankWidthMillimeters,
            UseKingPlank,
            (decimal)KingPlankWidthMillimeters);

        SetProperty(ref rowCount, calculatedRows, nameof(RowCount));
    }

    private void ValidateDeckContour()
    {
        DeckContourBuilder.Build(new DeckContourSettings(
            SelectedDeckShape.Value,
            (decimal)BowWidthPercentage,
            (decimal)SternWidthPercentage,
            (decimal)BowTaperLengthPercentage,
            (decimal)SternTaperLengthPercentage,
            (decimal)BowRoundnessPercentage,
            (decimal)SternRoundnessPercentage));
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
            PatternRows.Add(row with
            {
                SeamPositionsText = FormatSeamPositions(row.SeamPositionsMillimeters)
            });
        }
    }

    private static string FormatSeamPositions(IReadOnlyList<decimal> seamPositionsMillimeters)
    {
        return seamPositionsMillimeters.Count == 0
            ? "-"
            : string.Join(", ", seamPositionsMillimeters.Select(DisplayLengthFormatter.FormatRulerLabel));
    }
}
