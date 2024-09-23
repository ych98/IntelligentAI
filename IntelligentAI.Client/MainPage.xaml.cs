using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace IntelligentAI.Client;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();


        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }
}
