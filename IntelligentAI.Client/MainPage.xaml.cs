using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace IntelligentAI.Client;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // 修复安卓设备输入框遮挡 UI 的问题
        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }
}
