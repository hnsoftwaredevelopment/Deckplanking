using DeckPlanking.App.ViewModels;
using DeckPlanking.App.Graphics;
using System.Collections.Specialized;
using System.ComponentModel;

namespace DeckPlanking.App;

public partial class MainPage : ContentPage
{
    private readonly DeckPatternPreviewDrawable patternPreviewDrawable = new();
    private readonly ScaleInputViewModel viewModel = new();

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

        if (e.PropertyName is nameof(ScaleInputViewModel.UseKingPlank))
        {
            UpdatePatternPreview();
        }
    }

    private void UpdatePatternPreview()
    {
        patternPreviewDrawable.Rows = viewModel.PatternRows.ToArray();
        patternPreviewDrawable.DeckLengthMillimeters = viewModel.DeckLengthMillimeters;
        patternPreviewDrawable.SegmentLengthMillimeters = (double)viewModel.SegmentLengthMillimeters;
        patternPreviewDrawable.UseKingPlank = viewModel.UseKingPlank;
        PatternGraphics.Invalidate();
    }
}
