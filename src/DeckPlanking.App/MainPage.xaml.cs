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
    private DeckPlankingProjectSettings defaultProjectSettings = null!;
    private string? currentProjectFileName;
    private string? currentProjectFilePath;
    private string? currentProjectDisplayLocation;
    private bool hasUnsavedProjectChanges;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = viewModel;
        defaultProjectSettings = viewModel.CaptureProjectSettings();

        PatternGraphics.Drawable = patternPreviewDrawable;
        Loaded += OnPageLoaded;
        viewModel.PatternRows.CollectionChanged += OnPatternRowsChanged;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        LocalizationResourceManager.Instance.PropertyChanged += OnLocalizationChanged;
        AppPreferencesStore.PreferenceChanged += OnAppPreferenceChanged;
        UpdateProjectUi();
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

        if (e.PropertyName is nameof(ScaleInputViewModel.DeckLengthMillimeters)
            or nameof(ScaleInputViewModel.DeckLengthInput))
        {
            UpdatePatternPreview();
        }

        if (e.PropertyName is nameof(ScaleInputViewModel.DeckWidthMillimeters)
            or nameof(ScaleInputViewModel.DeckWidthInput)
            or nameof(ScaleInputViewModel.PlankWidthMillimeters)
            or nameof(ScaleInputViewModel.PlankWidthInput)
            or nameof(ScaleInputViewModel.KingPlankWidthMillimeters)
            or nameof(ScaleInputViewModel.KingPlankWidthInput)
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
            MarkProjectChanged();
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
        UpdateProjectUi();
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

    private async void OnPrintClicked(object? sender, EventArgs e)
    {
        try
        {
            var result = await PreviewPrinter.PrintAsync(patternPreviewDrawable);
            if (result == PrintResult.OpenedPdfForPrinting)
            {
                await DisplayAlertAsync(T("PrintPdfOpenedTitle"), T("PrintPdfOpenedMessage"), T("Ok"));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(T("PrintFailed"), ex.Message, T("Ok"));
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

    private async void OnNewProjectClicked(object? sender, EventArgs e)
    {
        if (!await ConfirmDiscardUnsavedChangesAsync())
        {
            return;
        }

        ApplyProjectSettings(defaultProjectSettings);
        currentProjectFileName = null;
        currentProjectFilePath = null;
        currentProjectDisplayLocation = null;
        hasUnsavedProjectChanges = false;
        UpdateProjectUi();
        await SaveLastProjectSettingsAsync();
    }

    private async void OnSaveProjectClicked(object? sender, EventArgs e)
    {
        await SaveProjectAsync();
    }

    private async void OnRenameProjectClicked(object? sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(currentProjectFileName))
            {
                await DisplayAlertAsync(T("ProjectRenameRequiresSaveTitle"), T("ProjectRenameRequiresSaveMessage"), T("Ok"));
                return;
            }

            var projectName = await PromptForProjectNameAsync(T("RenameProject"), currentProjectFileName);
            if (projectName is null)
            {
                return;
            }

            var renameResult = await ProjectFileService.RenameAsync(currentProjectFilePath, projectName);
            currentProjectFileName = renameResult.FileName;
            currentProjectFilePath = renameResult.FilePath;
            currentProjectDisplayLocation = renameResult.DisplayLocation;
            UpdateProjectUi();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(T("ProjectCouldNotBeRenamed"), ex.Message, T("Ok"));
        }
    }

    private async void OnDeleteProjectClicked(object? sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(currentProjectFileName))
            {
                await DisplayAlertAsync(T("ProjectDeleteRequiresSaveTitle"), T("ProjectDeleteRequiresSaveMessage"), T("Ok"));
                return;
            }

            var delete = await DisplayAlertAsync(
                T("DeleteProject"),
                string.Format(T("DeleteProjectConfirmation"), ProjectFileService.GetProjectDisplayName(currentProjectFileName)),
                T("DeleteProject"),
                T("Cancel"));
            if (!delete)
            {
                return;
            }

            await ProjectFileService.DeleteAsync(currentProjectFilePath);
            ApplyProjectSettings(defaultProjectSettings);
            currentProjectFileName = null;
            currentProjectFilePath = null;
            currentProjectDisplayLocation = null;
            hasUnsavedProjectChanges = false;
            UpdateProjectUi();
            await SaveLastProjectSettingsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(T("ProjectCouldNotBeDeleted"), ex.Message, T("Ok"));
        }
    }

    private async Task SaveProjectAsync()
    {
        try
        {
            var document = CreateCurrentProjectDocument();
            ProjectFileResult saveResult;

            if (string.IsNullOrWhiteSpace(currentProjectFileName)
                || string.IsNullOrWhiteSpace(currentProjectFilePath))
            {
                var projectName = await PromptForProjectNameAsync(T("SaveProject"), currentProjectFileName);
                if (projectName is null)
                {
                    return;
                }

                saveResult = await ProjectFileService.SaveNamedAsync(document, projectName);
            }
            else
            {
                saveResult = await ProjectFileService.SaveExistingAsync(document, currentProjectFilePath);
            }

            if (!saveResult.Saved)
            {
                return;
            }

            currentProjectFileName = string.IsNullOrWhiteSpace(saveResult.FileName)
                ? currentProjectFileName
                : saveResult.FileName;
            currentProjectFilePath = saveResult.FilePath ?? currentProjectFilePath;
            currentProjectDisplayLocation = string.IsNullOrWhiteSpace(saveResult.DisplayLocation)
                ? currentProjectDisplayLocation
                : saveResult.DisplayLocation;
            hasUnsavedProjectChanges = false;
            UpdateProjectUi();

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

    private async Task<string?> PromptForProjectNameAsync(string title, string? currentName)
    {
        var initialValue = string.IsNullOrWhiteSpace(currentName)
            ? T("UntitledProject")
            : ProjectFileService.GetProjectDisplayName(currentName);

        var projectName = await DisplayPromptAsync(
            title,
            T("ProjectNamePrompt"),
            T("Ok"),
            T("Cancel"),
            initialValue: initialValue,
            maxLength: 80);

        return string.IsNullOrWhiteSpace(projectName)
            ? null
            : projectName.Trim();
    }

    private async void OnOpenProjectClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!await ConfirmDiscardUnsavedChangesAsync())
            {
                return;
            }

            var openResult = await ProjectFileService.OpenProjectAsync();
            if (openResult is null)
            {
                return;
            }

            ApplyProjectSettings(openResult.Document.Settings);
            currentProjectFileName = openResult.FileName;
            currentProjectFilePath = openResult.FilePath;
            currentProjectDisplayLocation = openResult.DisplayLocation;
            hasUnsavedProjectChanges = false;
            UpdateProjectUi();
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

    private DeckPlankingProjectDocument CreateCurrentProjectDocument()
    {
        return DeckPlankingProjectDocument.Create(
            viewModel.CaptureProjectSettings(),
            DateTimeOffset.UtcNow);
    }

    private void MarkProjectChanged()
    {
        if (hasUnsavedProjectChanges)
        {
            return;
        }

        hasUnsavedProjectChanges = true;
        UpdateProjectUi();
    }

    private void UpdateProjectUi()
    {
        var projectName = string.IsNullOrWhiteSpace(currentProjectFileName)
            ? T("UntitledProject")
            : ProjectFileService.GetProjectDisplayName(currentProjectFileName);

        ProjectNameLabel.Text = hasUnsavedProjectChanges
            ? string.Format(T("UnsavedProjectName"), projectName)
            : projectName;

        ProjectStatusLabel.Text = hasUnsavedProjectChanges
            ? T("ProjectHasUnsavedChanges")
            : string.IsNullOrWhiteSpace(currentProjectDisplayLocation)
                ? T("ProjectNotSavedYet")
                : string.Format(T("ProjectSavedAt"), currentProjectDisplayLocation);
    }

    private async Task<bool> ConfirmDiscardUnsavedChangesAsync()
    {
        if (!hasUnsavedProjectChanges)
        {
            return true;
        }

        return await DisplayAlertAsync(
            T("UnsavedChangesTitle"),
            T("UnsavedChangesMessage"),
            T("DiscardChanges"),
            T("Cancel"));
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
            or nameof(ScaleInputViewModel.DeckLengthInput)
            or nameof(ScaleInputViewModel.DeckWidthMillimeters)
            or nameof(ScaleInputViewModel.DeckWidthInput)
            or nameof(ScaleInputViewModel.PlankWidthMillimeters)
            or nameof(ScaleInputViewModel.PlankWidthInput)
            or nameof(ScaleInputViewModel.KingPlankWidthMillimeters)
            or nameof(ScaleInputViewModel.KingPlankWidthInput)
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
