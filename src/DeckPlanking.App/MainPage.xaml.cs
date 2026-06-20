using DeckPlanking.App.ViewModels;
using DeckPlanking.App.Export;
using DeckPlanking.App.Graphics;
using DeckPlanking.App.Localization;
using DeckPlanking.App.Projects;
using DeckPlanking.App.Settings;
using DeckPlanking.Core.Preview;
using DeckPlanking.Core.Projects;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DeckPlanking.App;

public partial class MainPage : ContentPage
{
    private readonly DeckPatternPreviewDrawable patternPreviewDrawable = new();
    private readonly ScaleInputViewModel viewModel = new();
    private PreviewViewport previewViewport = PreviewViewport.Default;
    private double previousPanTotalX;
    private double previousPanTotalY;
    private double previousPinchScale = 1;
    private bool hasLoadedLastProjectSettings;
    private bool isApplyingProjectSettings;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = viewModel;

        PatternGraphics.Drawable = patternPreviewDrawable;
        Loaded += OnPageLoaded;
        viewModel.PatternRows.CollectionChanged += OnPatternRowsChanged;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        LocalizationResourceManager.Instance.PropertyChanged += OnLocalizationChanged;
        AppPreferencesStore.PreferenceChanged += OnAppPreferenceChanged;
        UpdatePatternPreview();
    }

    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        if (hasLoadedLastProjectSettings)
        {
            return;
        }

        hasLoadedLastProjectSettings = true;

        try
        {
            var settings = await LastProjectSettingsStore.LoadAsync();
            if (settings is null)
            {
                return;
            }

            ApplyProjectSettings(settings);
        }
        catch
        {
            // Last-used settings are a convenience; a bad autosave must not block the app.
        }
    }

    private void OnPatternRowsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdatePatternPreview();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (IsProjectSettingProperty(e.PropertyName))
        {
            ClearSegmentInspection();
        }

        if (e.PropertyName is nameof(ScaleInputViewModel.DeckLengthMillimeters))
        {
            UpdatePatternPreview();
        }

        if (e.PropertyName is nameof(ScaleInputViewModel.DeckWidthMillimeters)
            or nameof(ScaleInputViewModel.PlankWidthMillimeters)
            or nameof(ScaleInputViewModel.KingPlankWidthMillimeters)
            or nameof(ScaleInputViewModel.SelectedRowInputMode))
        {
            UpdatePatternPreview();
        }

        if (e.PropertyName is nameof(ScaleInputViewModel.SegmentLengthMillimeters))
        {
            UpdatePatternPreview();
        }

        if (e.PropertyName is nameof(ScaleInputViewModel.CutLengthMillimeters))
        {
            UpdatePatternPreview();
        }

        if (e.PropertyName is nameof(ScaleInputViewModel.UseKingPlank))
        {
            UpdatePatternPreview();
        }

        if (e.PropertyName is nameof(ScaleInputViewModel.RowCount)
            or nameof(ScaleInputViewModel.StartPoint)
            or nameof(ScaleInputViewModel.SelectedPattern)
            or nameof(ScaleInputViewModel.SelectedTrenailPattern)
            or nameof(ScaleInputViewModel.SelectedDeckOrientation))
        {
            UpdatePatternPreview();
        }

        if (!isApplyingProjectSettings && IsProjectSettingProperty(e.PropertyName))
        {
            _ = SaveLastProjectSettingsAsync();
        }
    }

    private void UpdatePatternPreview()
    {
        patternPreviewDrawable.Rows = viewModel.PatternRows.ToArray();
        patternPreviewDrawable.PatternKind = viewModel.SelectedPattern.Value;
        patternPreviewDrawable.RowsPerSide = viewModel.RowCount;
        patternPreviewDrawable.StartPoint = viewModel.StartPoint;
        patternPreviewDrawable.PlankLengthMillimeters = (double)viewModel.CutLengthMillimeters;
        patternPreviewDrawable.DeckLengthMillimeters = viewModel.DeckLengthMillimeters;
        patternPreviewDrawable.SegmentLengthMillimeters = (double)viewModel.SegmentLengthMillimeters;
        patternPreviewDrawable.PlankWidthMillimeters = viewModel.PlankWidthMillimeters;
        patternPreviewDrawable.KingPlankWidthMillimeters = viewModel.KingPlankWidthMillimeters;
        patternPreviewDrawable.UseKingPlank = viewModel.UseKingPlank;
        patternPreviewDrawable.ShowTrenails = viewModel.SelectedTrenailPattern.Value != TrenailPatternKind.None;
        patternPreviewDrawable.TrenailPatternKind = viewModel.SelectedTrenailPattern.Value;
        patternPreviewDrawable.DeckOrientation = viewModel.SelectedDeckOrientation.Value;
        patternPreviewDrawable.BowLabel = T("Bow");
        patternPreviewDrawable.SternLabel = T("Stern");
        patternPreviewDrawable.Zoom = previewViewport.Zoom;
        patternPreviewDrawable.PanX = previewViewport.PanX;
        patternPreviewDrawable.PanY = previewViewport.PanY;
        PatternGraphics.Invalidate();
    }

    private void OnZoomOutClicked(object? sender, EventArgs e)
    {
        previewViewport = previewViewport.ZoomBy(0.8);
        UpdatePatternPreview();
    }

    private void OnZoomInClicked(object? sender, EventArgs e)
    {
        previewViewport = previewViewport.ZoomBy(1.25);
        UpdatePatternPreview();
    }

    private void OnResetViewportClicked(object? sender, EventArgs e)
    {
        previewViewport = previewViewport.Reset();
        UpdatePatternPreview();
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private void OnPatternTapped(object? sender, TappedEventArgs e)
    {
        var position = e.GetPosition(PatternGraphics);
        if (position is null)
        {
            return;
        }

        var inspection = patternPreviewDrawable.HitTest(
            new PointF((float)position.Value.X, (float)position.Value.Y),
            new RectF(0, 0, (float)PatternGraphics.Width, (float)PatternGraphics.Height));

        patternPreviewDrawable.SelectedSegment = inspection;
        SegmentInspectionLabel.Text = inspection is null
            ? string.Empty
            : $"{inspection.RowLabel}, segment {inspection.SegmentNumber}: {DisplayLengthFormatter.Format(inspection.StartMillimeters)} - {DisplayLengthFormatter.Format(inspection.EndMillimeters)}, length {DisplayLengthFormatter.Format(inspection.LengthMillimeters)}";
        SegmentInspectionLabel.IsVisible = inspection is not null;
        PatternGraphics.Invalidate();
    }

    private void OnLocalizationChanged(object? sender, PropertyChangedEventArgs e)
    {
        UpdatePatternPreview();
    }

    private void OnAppPreferenceChanged(object? sender, AppPreferencesChangedEventArgs e)
    {
        if (e.PreferenceName == AppPreferencesStore.DisplayUnitSystemPreferenceName)
        {
            ClearSegmentInspection();
            UpdatePatternPreview();
        }
    }

    private void ClearSegmentInspection()
    {
        patternPreviewDrawable.SelectedSegment = null;
        SegmentInspectionLabel.Text = string.Empty;
        SegmentInspectionLabel.IsVisible = false;
    }

    private async void OnExportPngClicked(object? sender, EventArgs e)
    {
        try
        {
            var fileResult = await PreviewPngExporter.ExportAsync(patternPreviewDrawable);
            await SaveExportAsync(fileResult);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(T("ExportFailed"), ex.Message, T("Ok"));
        }
    }

    private async void OnExportPdfClicked(object? sender, EventArgs e)
    {
        try
        {
            var fileResult = await PreviewPdfExporter.ExportAsync(patternPreviewDrawable);
            await SaveExportAsync(fileResult);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(T("ExportFailed"), ex.Message, T("Ok"));
        }
    }

    private async Task SaveExportAsync(FileResult fileResult)
    {
        var saveResult = await ExportFileSaver.SaveAsync(fileResult);
        if (!saveResult.Saved)
        {
            return;
        }

        await DisplayAlertAsync(
            T("ExportSaved"),
            string.Format(T("SavedFileToLocation"), saveResult.FileName, saveResult.DisplayLocation),
            T("Ok"));
    }

    private async void OnSaveProjectClicked(object? sender, EventArgs e)
    {
        try
        {
            var document = DeckPlankingProjectDocument.Create(
                viewModel.CaptureProjectSettings(),
                DateTimeOffset.UtcNow);

            var saveResult = await ProjectFileService.SaveAsync(document);
            if (!saveResult.Saved)
            {
                return;
            }

            await DisplayAlertAsync(
                T("ProjectSaved"),
                string.Format(T("SavedFileToLocation"), saveResult.FileName, saveResult.DisplayLocation),
                T("Ok"));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(T("ProjectCouldNotBeSaved"), ex.Message, T("Ok"));
        }
    }

    private async void OnOpenProjectClicked(object? sender, EventArgs e)
    {
        try
        {
            var document = await ProjectFileService.OpenAsync();
            if (document is null)
            {
                return;
            }

            ApplyProjectSettings(document.Settings);
            await SaveLastProjectSettingsAsync();

            await DisplayAlertAsync(
                T("ProjectOpened"),
                T("ProjectOpenedMessage"),
                T("Ok"));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(T("ProjectCouldNotBeOpened"), ex.Message, T("Ok"));
        }
    }

    private void ApplyProjectSettings(DeckPlankingProjectSettings settings)
    {
        isApplyingProjectSettings = true;
        try
        {
            viewModel.ApplyProjectSettings(settings);
            previewViewport = PreviewViewport.Default;
            UpdatePatternPreview();
        }
        finally
        {
            isApplyingProjectSettings = false;
        }
    }

    private async Task SaveLastProjectSettingsAsync()
    {
        try
        {
            await LastProjectSettingsStore.SaveAsync(viewModel.CaptureProjectSettings());
        }
        catch
        {
            // Last-used settings are best-effort and should never interrupt editing.
        }
    }

    private static bool IsProjectSettingProperty(string? propertyName)
    {
        return propertyName is nameof(ScaleInputViewModel.RealPlankLength)
            or nameof(ScaleInputViewModel.SelectedLengthUnit)
            or nameof(ScaleInputViewModel.SelectedScaleMode)
            or nameof(ScaleInputViewModel.DecimalScale)
            or nameof(ScaleInputViewModel.ImperialInchesPerFoot)
            or nameof(ScaleInputViewModel.DeckLengthMillimeters)
            or nameof(ScaleInputViewModel.DeckWidthMillimeters)
            or nameof(ScaleInputViewModel.PlankWidthMillimeters)
            or nameof(ScaleInputViewModel.KingPlankWidthMillimeters)
            or nameof(ScaleInputViewModel.SelectedRowInputMode)
            or nameof(ScaleInputViewModel.RowCount)
            or nameof(ScaleInputViewModel.StartPoint)
            or nameof(ScaleInputViewModel.SelectedPattern)
            or nameof(ScaleInputViewModel.UseKingPlank)
            or nameof(ScaleInputViewModel.SelectedDeckOrientation)
            or nameof(ScaleInputViewModel.SelectedTrenailPattern);
    }

    private static string T(string key)
    {
        return LocalizationResourceManager.Instance[key];
    }

    private void OnPatternPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (e.StatusType == GestureStatus.Started)
        {
            previousPanTotalX = 0;
            previousPanTotalY = 0;
            return;
        }

        if (e.StatusType == GestureStatus.Running)
        {
            var deltaX = e.TotalX - previousPanTotalX;
            var deltaY = e.TotalY - previousPanTotalY;
            previousPanTotalX = e.TotalX;
            previousPanTotalY = e.TotalY;

            previewViewport = previewViewport.PanBy(deltaX, deltaY);
            UpdatePatternPreview();
        }
    }

    private void OnPatternPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Started)
        {
            previousPinchScale = 1;
            return;
        }

        if (e.Status == GestureStatus.Running)
        {
            var factor = e.Scale / previousPinchScale;
            previousPinchScale = e.Scale;

            previewViewport = previewViewport.ZoomBy(factor);
            UpdatePatternPreview();
        }
    }
}
