using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;

namespace IntelligentAI.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("SegoeUI-Regular.ttf", "Segoe UI");
            });

#if DEBUG
        builder.Configuration.AddInMemoryCollection(AspireAppSettings.Settings);
#endif

        builder.Configuration.AddInMemoryCollection(AppSettings.Settings);

        builder.AddAppDefaults();

        builder.AddCustomServices();

        builder.Services.AddFluentUIComponents();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<MainPage>();

        MauiApp mauiApp = builder.Build();
        mauiApp.InitOpenTelemetryServices();

        // Set this switch to use the LEGACY behavior of always using 0.0.0.0 to host BlazorWebView
        //AppContext.SetSwitch("BlazorWebView.AppHostAddressAlways0000", true);

        return mauiApp;

    }
}
