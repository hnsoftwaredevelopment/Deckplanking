using System.Globalization;
using DeckPlanking.App.Feedback;
using DeckPlanking.App.Localization;
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

        feedbackTypes =
        [
            new(T("FeedbackTypeBug"), FeedbackSubmissionType.Bug),
            new(T("FeedbackTypeFeatureRequest"), FeedbackSubmissionType.FeatureRequest)
        ];

        FeedbackTypePicker.ItemsSource = feedbackTypes;
        FeedbackTypePicker.SelectedItem = feedbackTypes[0];

        VersionLabel.Text = AboutPage.GetApplicationVersion();
        PlatformLabel.Text = DeviceInfo.Current.Platform.ToString();
        OsVersionLabel.Text = DeviceInfo.Current.VersionString;
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
            new FeedbackApplicationContext(
                AppInfo.Current.Name,
                VersionLabel.Text,
                PlatformLabel.Text,
                OsVersionLabel.Text,
                CultureInfo.CurrentUICulture.Name));

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

            StatusLabel.Text = string.IsNullOrWhiteSpace(result.IssueUrl)
                ? T("FeedbackSent")
                : string.Format(CultureInfo.CurrentCulture, T("FeedbackSentWithIssue"), result.IssueUrl);
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
}
