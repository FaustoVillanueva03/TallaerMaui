using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class SeriesPage : BasePage
{
    private readonly SerieService _serieService;

    private bool _seriesCargadas;
    private Button? _categoriaSeleccionada;

    public SeriesPage()
    {
        InitializeComponent();

        _serieService =
            new SerieService();

        _categoriaSeleccionada =
            TodasButton;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_seriesCargadas)
        {
            return;
        }

        await CargarSeriesPopularesAsync();
    }

    private async Task CargarSeriesPopularesAsync()
    {
        try
        {
            MostrarCargando(true);

            List<Serie> series =
                await _serieService
                    .ObtenerSeriesAsync();

            ListaSeries.ItemsSource =
                series;

            _seriesCargadas = true;
        }
        catch (Exception error)
        {
            await DisplayAlert(
                "Error",
                $"No se pudieron cargar las series.\n\n{error.Message}",
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
            await CargarSeriesPopularesAsync();
            return;
        }

        await CargarSeriesPorCategoriaAsync(
            generoId);
    }

    private void SeleccionarBotonCategoria(
        Button boton)
    {
        Color primario =
            (Color)Application.Current!
                .Resources["ColorPrimario"];

        Color superficie =
            (Color)Application.Current!
                .Resources["ColorSuperficie"];

        Color texto =
            (Color)Application.Current!
                .Resources["ColorTextoPrincipal"];

        if (_categoriaSeleccionada != null)
        {
            _categoriaSeleccionada.BackgroundColor =
                superficie;

            _categoriaSeleccionada.TextColor =
                primario;
        }

        boton.BackgroundColor =
            primario;

        boton.TextColor =
            texto;

        _categoriaSeleccionada =
            boton;
    }

    private async Task CargarSeriesPorCategoriaAsync(
        int generoId)
    {
        try
        {
            MostrarCargando(true);

            List<Serie> series =
                await _serieService
                    .ObtenerSeriesPorGeneroAsync(
                        generoId,
                        20);

            ListaSeries.ItemsSource =
                series;
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
        await BuscarSeriesAsync();
    }

    private async void OnBuscarCompleted(
        object sender,
        EventArgs e)
    {
        await BuscarSeriesAsync();
    }

    private async Task BuscarSeriesAsync()
    {
        string texto =
            BusquedaEntry.Text?.Trim() ??
            string.Empty;

        if (string.IsNullOrWhiteSpace(texto))
        {
            SeleccionarBotonCategoria(
                TodasButton);

            await CargarSeriesPopularesAsync();

            return;
        }

        try
        {
            MostrarCargando(true);

            List<Serie> resultados =
                await _serieService
                    .BuscarSeriesAsync(
                        texto);

            ListaSeries.ItemsSource =
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

        ListaSeries.IsVisible =
            !mostrar;
    }

    private async void OnSerieSeleccionada(
        object sender,
        SelectionChangedEventArgs e)
    {
        Serie? serie =
            e.CurrentSelection
                .FirstOrDefault()
                as Serie;

        if (serie == null)
        {
            return;
        }

        if (sender is CollectionView lista)
        {
            lista.SelectedItem = null;
        }

        await Navigation.PushAsync(
            new DetalleSeriePage(
                serie));
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}