using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class ClimaPage : BasePage
{
    private readonly WeatherService _weatherService;
    private readonly MovieService _movieService;
    private readonly SerieService _serieService;

    private bool _datosCargados;

    private int _indicePeliculas;
    private int _indiceSeries;

    private int _generoPeliculaClima;
    private int _generoSerieClima;

    public ClimaPage()
    {
        InitializeComponent();

        _weatherService = new WeatherService();
        _movieService = new MovieService();
        _serieService = new SerieService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_datosCargados)
        {
            return;
        }

        try
        {
            Cargando.IsVisible = true;
            Cargando.IsRunning = true;

            RespuestaClima? clima =
                await _weatherService.ObtenerClimaActualAsync();

            if (clima == null ||
                clima.Weather == null ||
                clima.Weather.Count == 0)
            {
                await DisplayAlert(
                    "Clima",
                    "No fue posible obtener el clima actual.",
                    "Aceptar");

                return;
            }

            string descripcion =
                clima.Weather[0].Descripcion;

            CiudadLabel.Text =
                clima.Ciudad;

            TemperaturaLabel.Text =
                $"{clima.Main.Temperatura:0}°C";

            DescripcionLabel.Text =
                PrimeraLetraMayuscula(descripcion);

            SensacionLabel.Text =
                $"Sensación térmica: " +
                $"{clima.Main.SensacionTermica:0}°C";

            HumedadLabel.Text =
                $"Humedad: {clima.Main.Humedad}%";

            IconoImage.Source =
                $"https://openweathermap.org/img/wn/" +
                $"{clima.Weather[0].Icono}@2x.png";

            List<PronosticoDia> pronostico =
                await _weatherService
                    .ObtenerPronostico5DiasAsync();

            ListaPronostico.ItemsSource =
                pronostico.Take(5).ToList();

            var recomendacion =
                ObtenerGenerosSegunClima(descripcion);

            _generoPeliculaClima =
                recomendacion.GeneroPelicula;

            _generoSerieClima =
                recomendacion.GeneroSerie;

            MotivoRecomendacionLabel.Text =
                recomendacion.Motivo;

            Task<List<Pelicula>> tareaPeliculas =
                _movieService
                    .ObtenerPeliculasPorGeneroAsync(
                        _generoPeliculaClima,
                        20);

            Task<List<Serie>> tareaSeries =
                _serieService
                    .ObtenerSeriesPorGeneroAsync(
                        _generoSerieClima,
                        20);

            await Task.WhenAll(
                tareaPeliculas,
                tareaSeries);

            ListaPeliculasClima.ItemsSource =
                await tareaPeliculas;

            ListaSeriesClima.ItemsSource =
                await tareaSeries;

            _indicePeliculas = 0;
            _indiceSeries = 0;
            _datosCargados = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                ex.Message,
                "Aceptar");
        }
        finally
        {
            Cargando.IsRunning = false;
            Cargando.IsVisible = false;
        }
    }

    private static (
        int GeneroPelicula,
        int GeneroSerie,
        string Motivo)
        ObtenerGenerosSegunClima(
            string descripcion)
    {
        descripcion =
            descripcion.ToLowerInvariant();

        if (descripcion.Contains("lluvia") ||
            descripcion.Contains("llovizna") ||
            descripcion.Contains("tormenta"))
        {
            return (
                53,
                9648,
                "Como está lluvioso, te recomendamos " +
                "películas de suspenso y series de misterio.");
        }

        if (descripcion.Contains("nube") ||
            descripcion.Contains("niebla") ||
            descripcion.Contains("neblina"))
        {
            return (
                9648,
                9648,
                "El día está nublado: es un buen momento " +
                "para disfrutar historias de misterio.");
        }

        if (descripcion.Contains("despejado") ||
            descripcion.Contains("sol"))
        {
            return (
                35,
                35,
                "Como el día está despejado, elegimos " +
                "comedias para acompañar el buen clima.");
        }

        if (descripcion.Contains("nieve") ||
            descripcion.Contains("frío") ||
            descripcion.Contains("frio"))
        {
            return (
                878,
                10765,
                "Para este clima frío elegimos ciencia " +
                "ficción y fantasía.");
        }

        return (
            12,
            10759,
            "Para el clima de hoy te recomendamos " +
            "películas y series de aventura.");
    }

    private static string PrimeraLetraMayuscula(
        string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return string.Empty;
        }

        return char.ToUpper(texto[0]) +
               texto.Substring(1);
    }

    private void OnPeliculasAnteriorClicked(
        object sender,
        EventArgs e)
    {
        if (_indicePeliculas <= 0)
        {
            return;
        }

        _indicePeliculas--;

        ListaPeliculasClima.ScrollTo(
            _indicePeliculas,
            position: ScrollToPosition.Start,
            animate: true);
    }

    private void OnPeliculasSiguienteClicked(
        object sender,
        EventArgs e)
    {
        if (ListaPeliculasClima.ItemsSource
            is not IEnumerable<Pelicula> elementos)
        {
            return;
        }

        List<Pelicula> peliculas =
            elementos.ToList();

        if (_indicePeliculas >= peliculas.Count - 1)
        {
            return;
        }

        _indicePeliculas++;

        ListaPeliculasClima.ScrollTo(
            _indicePeliculas,
            position: ScrollToPosition.Start,
            animate: true);
    }

    private void OnSeriesAnteriorClicked(
        object sender,
        EventArgs e)
    {
        if (_indiceSeries <= 0)
        {
            return;
        }

        _indiceSeries--;

        ListaSeriesClima.ScrollTo(
            _indiceSeries,
            position: ScrollToPosition.Start,
            animate: true);
    }

    private void OnSeriesSiguienteClicked(
        object sender,
        EventArgs e)
    {
        if (ListaSeriesClima.ItemsSource
            is not IEnumerable<Serie> elementos)
        {
            return;
        }

        List<Serie> series =
            elementos.ToList();

        if (_indiceSeries >= series.Count - 1)
        {
            return;
        }

        _indiceSeries++;

        ListaSeriesClima.ScrollTo(
            _indiceSeries,
            position: ScrollToPosition.Start,
            animate: true);
    }

    private async void OnPeliculaSeleccionada(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault()
            is not Pelicula peliculaSeleccionada)
        {
            return;
        }

        if (sender is CollectionView lista)
        {
            lista.SelectedItem = null;
        }

        await Navigation.PushAsync(
            new DetallePeliculaPage(
                peliculaSeleccionada));
    }

    private async void OnSerieSeleccionada(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault()
            is not Serie serieSeleccionada)
        {
            return;
        }

        if (sender is CollectionView lista)
        {
            lista.SelectedItem = null;
        }

        await Navigation.PushAsync(
            new DetalleSeriePage(
                serieSeleccionada));
    }

    private async void OnVerPeliculasClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new PeliculasPage());
    }

    private async void OnVerSeriesClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new SeriesPage());
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}