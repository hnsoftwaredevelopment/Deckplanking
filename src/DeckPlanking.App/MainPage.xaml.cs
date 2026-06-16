using DeckPlanking.App.ViewModels;

namespace DeckPlanking.App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new ScaleInputViewModel();
    }
}
