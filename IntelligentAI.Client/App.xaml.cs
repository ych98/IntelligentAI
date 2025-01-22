namespace IntelligentAI.Client;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

#if !WINDOWS
        MainPage = new AppShell();
#endif
    }

#if WINDOWS
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new MainWindow();
    }
#endif

}
