namespace MAUINavegacion;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        return new Window(
            new NavigationPage(new LoginPage())
            {
                BarBackgroundColor = Color.FromArgb("#151515"),
                BarTextColor = Colors.White
            });
    }
}