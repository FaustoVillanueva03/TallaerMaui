using MAUINavegacion.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace MAUINavegacion;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder =
            MauiApp.CreateBuilder();

        builder.UseMauiApp<App>();

#if !WINDOWS
        builder.UseMauiMaps();
#endif

#if ANDROID
        builder.ConfigureMauiHandlers(
            handlers =>
            {
                handlers.AddHandler(
                    typeof(YoutubeWebView),
                    typeof(YoutubeWebViewHandler));
            });
#endif

#if WINDOWS
        builder.ConfigureMauiHandlers(
            handlers =>
            {
                handlers.AddHandler(
                    typeof(YoutubeWebView),
                    typeof(YoutubeWebViewHandlerWindows));
            });
#endif

        builder.ConfigureFonts(
            fonts =>
            {
                fonts.AddFont(
                    "OpenSans-Regular.ttf",
                    "OpenSansRegular");

                fonts.AddFont(
                    "OpenSans-Semibold.ttf",
                    "OpenSansSemibold");
            });

        string dbPath =
            Path.Combine(
                FileSystem.AppDataDirectory,
                "redflix.db3");

        builder.Services.AddSingleton(
            new AppDatabase(dbPath));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}