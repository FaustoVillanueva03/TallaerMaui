using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class DetallePeliculaPage : ContentPage
{
    private readonly MovieService _movieService;
    private Pelicula? _pelicula;
    private bool _trailerCargado;

    public DetallePeliculaPage()
    {
        InitializeComponent();

        _movieService = new MovieService();
    }

    public DetallePeliculaPage(Pelicula pelicula) : this()
    {
        _pelicula = pelicula;
        BindingContext = pelicula;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_trailerCargado || _pelicula == null)
            return;

        _trailerCargado = true;

        await CargarTrailerAsync();
    }

    private async Task CargarTrailerAsync()
    {
        try
        {
            Trailer? trailer =
                await _movieService.ObtenerTrailerAsync(_pelicula!.Id);

            if (trailer == null ||
                string.IsNullOrWhiteSpace(trailer.Key))
            {
                MensajeSinTrailer.Text =
                    "No hay un tráiler disponible para esta película.";

                MensajeSinTrailer.IsVisible = true;
                SeccionTrailer.IsVisible = false;

                return;
            }

            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta name=""viewport""
          content=""width=device-width, initial-scale=1.0"">

    <style>
        html, body {{
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
            background-color: black;
            overflow: hidden;
        }}

        iframe {{
            width: 100%;
            height: 100%;
            border: 0;
        }}
    </style>
</head>

<body>
    <iframe
        width=""100%""
        height=""100%""
        src=""https://www.youtube-nocookie.com/embed/{{trailer.Key}}?playsinline=1&rel=0""
        title=""YouTube video player""
        frameborder=""0""
        allow=""accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share""
        allowfullscreen>
    </iframe>
</body>
</html>";

            TrailerWebView.Source = new HtmlWebViewSource
            {
                Html = html
            };

            MensajeSinTrailer.IsVisible = false;
            SeccionTrailer.IsVisible = true;
        }
        catch (Exception)
        {
            MensajeSinTrailer.Text =
                "No se pudo cargar el tráiler.";

            MensajeSinTrailer.IsVisible = true;
            SeccionTrailer.IsVisible = false;
        }
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }
}