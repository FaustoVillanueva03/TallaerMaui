using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class PeliculasPage : BasePage
{
    private readonly MovieService _movieService;

    private bool _peliculasCargadas;
    private Button? _categoriaSeleccionada;

    public PeliculasPage()
    {
        InitializeComponent();

        _movieService =
            new MovieService();

        _categoriaSeleccionada =
            TodasButton;
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
            MostrarCargando(true);

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
            MostrarCargando(false);
        }
    }

    private async void OnCategoriaClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button boton)
        {
            return;
        }

        SeleccionarBotonCategoria(
            boton);

        BusquedaEntry.Text =
            string.Empty;

        string parametro =
            boton.CommandParameter?
                .ToString() ??
            "0";

        if (!int.TryParse(
                parametro,
                out int generoId))
        {
            return;
        }

        if (generoId == 0)
        {
            await CargarPeliculasPopularesAsync();
            return;
        }

        await CargarPeliculasPorCategoriaAsync(
            generoId);
    }

    private void SeleccionarBotonCategoria(
        Button boton)
    {
        Color colorPrimario =
            (Color)Application.Current!
                .Resources["ColorPrimario"];

        Color colorSuperficie =
            (Color)Application.Current!
                .Resources["ColorSuperficie"];

        Color colorTextoPrincipal =
            (Color)Application.Current!
                .Resources["ColorTextoPrincipal"];

        if (_categoriaSeleccionada != null)
        {
            _categoriaSeleccionada.BackgroundColor =
                colorSuperficie;

            _categoriaSeleccionada.TextColor =
                colorPrimario;

            _categoriaSeleccionada.BorderColor =
                (Color)Application.Current!
                    .Resources["ColorBorde"];
        }

        boton.BackgroundColor =
            colorPrimario;

        boton.TextColor =
            colorTextoPrincipal;

        boton.BorderColor =
            colorPrimario;

        _categoriaSeleccionada =
            boton;
    }

    private async Task CargarPeliculasPorCategoriaAsync(
        int generoId)
    {
        try
        {
            MostrarCargando(true);

            List<Pelicula> peliculas =
                await _movieService
                    .ObtenerPeliculasPorGeneroAsync(
                        generoId,
                        20);

            ListaPeliculas.ItemsSource =
                peliculas;
        }
        catch (Exception error)
        {
            await DisplayAlert(
                "Error",
                $"No se pudo cargar la categoría.\n\n{error.Message}",
                "Aceptar");
        }
        finally
        {
            MostrarCargando(false);
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

        if (string.IsNullOrWhiteSpace(
                texto))
        {
            SeleccionarBotonCategoria(
                TodasButton);

            await CargarPeliculasPopularesAsync();

            return;
        }

        try
        {
            MostrarCargando(true);

            List<Pelicula> resultados =
                await _movieService
                    .BuscarPeliculasAsync(
                        texto);

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
            MostrarCargando(false);
        }
    }

    private void MostrarCargando(
        bool mostrar)
    {
        Cargando.IsVisible =
            mostrar;

        Cargando.IsRunning =
            mostrar;

        ListaPeliculas.IsVisible =
            !mostrar;
    }

    private async void OnPeliculaSeleccionada(
        object sender,
        SelectionChangedEventArgs e)
    {
        Pelicula? pelicula =
            e.CurrentSelection
                .FirstOrDefault()
                as Pelicula;

        if (pelicula == null)
        {
            return;
        }

        if (sender is CollectionView lista)
        {
            lista.SelectedItem = null;
        }

        await Navigation.PushAsync(
            new DetallePeliculaPage(
                pelicula));
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}