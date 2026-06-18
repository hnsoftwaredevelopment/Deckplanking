using DeckPlanking.App.ViewModels;
using DeckPlanking.App.Export;
using DeckPlanking.App.Graphics;
using DeckPlanking.Core.Preview;
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

    public MainPage()
    {
        InitializeComponent();
        BindingContext = viewModel;

        PatternGraphics.Drawable = patternPreviewDrawable;
        viewModel.PatternRows.CollectionChanged += OnPatternRowsChanged;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        UpdatePatternPreview();
    }

    private void OnPatternRowsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdatePatternPreview();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ScaleInputViewModel.DeckLengthMillimeters))
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
        patternPreviewDrawable.UseKingPlank = viewModel.UseKingPlank;
        patternPreviewDrawable.ShowTrenails = viewModel.SelectedTrenailPattern.Value != TrenailPatternKind.None;
        patternPreviewDrawable.TrenailPatternKind = viewModel.SelectedTrenailPattern.Value;
        patternPreviewDrawable.DeckOrientation = viewModel.SelectedDeckOrientation.Value;
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

    private async void OnExportPngClicked(object? sender, EventArgs e)
    {
        try
        {
            var fileResult = await PreviewPngExporter.ExportAsync(patternPreviewDrawable);
            var saveResult = await ExportFileSaver.SaveAsync(fileResult);
            if (saveResult.Saved)
            {
                await DisplayAlertAsync(
                    "Export saved",
                    $"Saved {saveResult.FileName} to {saveResult.DisplayLocation}.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Export failed", ex.Message, "OK");
        }
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
