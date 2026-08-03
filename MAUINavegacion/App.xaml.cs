using MAUINavegacion.Data;

namespace MAUINavegacion;

public partial class App : Application
{
    public static AppDatabase Database { get; private set; } = null!;

    public App(AppDatabase database)
    {
        InitializeComponent();

        Database = database;
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        return new Window(
            new NavigationPage(new LoginPage())
            {
                BarBackgroundColor =
                    Color.FromArgb("#151515"),

                BarTextColor = Colors.White
            });
    }
}