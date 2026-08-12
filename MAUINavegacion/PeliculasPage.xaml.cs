using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class PeliculasPage : BasePage
{
    private readonly MovieService _movieService;

    private bool _peliculasCargadas;

    public PeliculasPage()
    {
        InitializeComponent();

        _movieService = new MovieService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_peliculasCargadas)
        {
            return;
        }

        await CargarPeliculasPopularesAsync();
    }

    private async Task CargarPeliculasPopularesAsync()
    {
        try
        {
            Cargando.IsVisible = true;
            Cargando.IsRunning = true;
            ListaPeliculas.IsVisible = false;

            List<Pelicula> peliculas =
                await _movieService
                    .ObtenerPeliculasAsync();

            ListaPeliculas.ItemsSource =
                peliculas;

            _peliculasCargadas = true;
        }
        catch (Exception error)
        {
            await DisplayAlert(
                "Error",
                $"No se pudieron cargar las películas.\n\n{error.Message}",
                "Aceptar");
        }
        finally
        {
            Cargando.IsVisible = false;
            Cargando.IsRunning = false;
            ListaPeliculas.IsVisible = true;
        }
    }

    private async void OnBuscarClicked(
        object sender,
        EventArgs e)
    {
        await BuscarPeliculasAsync();
    }

    private async void OnBuscarCompleted(
        object sender,
        EventArgs e)
    {
        await BuscarPeliculasAsync();
    }

    private async Task BuscarPeliculasAsync()
    {
        string texto =
            BusquedaEntry.Text?.Trim() ??
            string.Empty;

        if (string.IsNullOrWhiteSpace(texto))
        {
            await CargarPeliculasPopularesAsync();
            return;
        }

        try
        {
            Cargando.IsVisible = true;
            Cargando.IsRunning = true;
            ListaPeliculas.IsVisible = false;

            List<Pelicula> resultados =
                await _movieService
                    .BuscarPeliculasAsync(texto);

            ListaPeliculas.ItemsSource =
                resultados;
        }
        catch (Exception error)
        {
            await DisplayAlert(
                "Error",
                $"No se pudo realizar la búsqueda.\n\n{error.Message}",
                "Aceptar");
        }
        finally
        {
            Cargando.IsVisible = false;
            Cargando.IsRunning = false;
            ListaPeliculas.IsVisible = true;
        }
    }

    private async void OnPeliculaSeleccionada(
        object sender,
        SelectionChangedEventArgs e)
    {
        Pelicula? peliculaSeleccionada =
            e.CurrentSelection
                .FirstOrDefault() as Pelicula;

        if (peliculaSeleccionada == null)
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

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}