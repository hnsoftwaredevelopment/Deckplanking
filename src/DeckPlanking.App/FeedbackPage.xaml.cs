using System.Globalization;
using System.Runtime.InteropServices;
using DeckPlanking.App.Feedback;
using DeckPlanking.App.Localization;
using DeckPlanking.App.Settings;
using DeckPlanking.App.ViewModels;
using DeckPlanking.Core.Feedback;

namespace DeckPlanking.App;

public partial class FeedbackPage : ContentPage
{
    private readonly List<OptionItem<FeedbackSubmissionType>> feedbackTypes;
    private readonly FeedbackWorkerClient feedbackWorkerClient = new();

    public FeedbackPage()
    {
        InitializeComponent();

        VersionLabel.Text = AboutPage.GetApplicationVersion();
        PlatformLabel.Text = DeviceInfo.Current.Platform.ToString();
        OsVersionLabel.Text = DeviceInfo.Current.VersionString;

        feedbackTypes =
        [
            new(T("FeedbackTypeBug"), FeedbackSubmissionType.Bug),
            new(T("FeedbackTypeFeatureRequest"), FeedbackSubmissionType.FeatureRequest)
        ];

        FeedbackTypePicker.ItemsSource = feedbackTypes;
        FeedbackTypePicker.SelectedItem = feedbackTypes[0];

        RefreshDiagnosticsVisibility();
    }

    private async void OnSendFeedbackClicked(object? sender, EventArgs e)
    {
        if (FeedbackTypePicker.SelectedItem is not OptionItem<FeedbackSubmissionType> selectedType)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || string.IsNullOrWhiteSpace(DescriptionEditor.Text))
        {
            StatusLabel.Text = T("FeedbackRequiredFields");
            return;
        }

        var submission = new FeedbackSubmission(
            selectedType.Value,
            TitleEntry.Text,
            DescriptionEditor.Text,
            NameEntry.Text,
            ContactEntry.Text,
            CreateApplicationContext(),
            selectedType.Value == FeedbackSubmissionType.Bug ? CreateDiagnostics() : null);

        SendFeedbackButton.IsEnabled = false;
        StatusLabel.Text = T("FeedbackSending");

        try
        {
            var result = await feedbackWorkerClient.SendAsync(submission);
            if (!result.IsSuccess)
            {
                StatusLabel.Text = string.Format(CultureInfo.CurrentCulture, T("FeedbackSendFailed"), result.ErrorMessage);
                return;
            }

            StatusLabel.Text = T("FeedbackSent");
        }
        finally
        {
            SendFeedbackButton.IsEnabled = true;
        }
    }

    private static string T(string key)
    {
        return LocalizationResourceManager.Instance[key];
    }

    private void OnFeedbackTypeChanged(object? sender, EventArgs e)
    {
        RefreshDiagnosticsVisibility();
    }

    private void OnDiagnosticsToggleClicked(object? sender, EventArgs e)
    {
        DiagnosticsPanel.IsVisible = !DiagnosticsPanel.IsVisible;
        DiagnosticsToggleButton.Text = DiagnosticsPanel.IsVisible
            ? T("HideTechnicalInformation")
            : T("ShowTechnicalInformation");
    }

    private void RefreshDiagnosticsVisibility()
    {
        var isBug = FeedbackTypePicker.SelectedItem is OptionItem<FeedbackSubmissionType> selectedType
            && selectedType.Value == FeedbackSubmissionType.Bug;

        DiagnosticsToggleButton.IsVisible = isBug;
        DiagnosticsPanel.IsVisible = false;
        DiagnosticsToggleButton.Text = T("ShowTechnicalInformation");

        if (isBug)
        {
            DiagnosticsLabel.Text = FeedbackDiagnosticsFormatter.Format(CreateApplicationContext(), CreateDiagnostics());
        }
    }

    private FeedbackApplicationContext CreateApplicationContext()
    {
        return new FeedbackApplicationContext(
            AppInfo.Current.Name,
            VersionLabel.Text,
            PlatformLabel.Text,
            OsVersionLabel.Text,
            CultureInfo.CurrentUICulture.Name);
    }

    private static FeedbackDiagnostics CreateDiagnostics()
    {
        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        return new FeedbackDiagnostics(
            RuntimeInformation.ProcessArchitecture.ToString(),
            DeviceInfo.Current.Idiom.ToString(),
            AppPreferencesStore.GetDisplayUnitSystem().ToString(),
            AppPreferencesStore.GetTheme().ToString(),
            string.Format(
                CultureInfo.InvariantCulture,
                "{0:0}x{1:0} @ {2:0.##}",
                displayInfo.Width,
                displayInfo.Height,
                displayInfo.Density));
    }
}
