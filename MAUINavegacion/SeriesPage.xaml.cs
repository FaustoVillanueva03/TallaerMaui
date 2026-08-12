using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class SeriesPage : BasePage
{
    private readonly SerieService _serieService;

    private bool _seriesCargadas;

    public SeriesPage()
    {
        InitializeComponent();

        _serieService = new SerieService();
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
            Cargando.IsVisible = true;
            Cargando.IsRunning = true;
            ListaSeries.IsVisible = false;

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
            Cargando.IsVisible = false;
            Cargando.IsRunning = false;
            ListaSeries.IsVisible = true;
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
            await CargarSeriesPopularesAsync();
            return;
        }

        try
        {
            Cargando.IsVisible = true;
            Cargando.IsRunning = true;
            ListaSeries.IsVisible = false;

            List<Serie> resultados =
                await _serieService
                    .BuscarSeriesAsync(texto);

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
            Cargando.IsVisible = false;
            Cargando.IsRunning = false;
            ListaSeries.IsVisible = true;
        }
    }

    private async void OnSerieSeleccionada(
        object sender,
        SelectionChangedEventArgs e)
    {
        Serie? serieSeleccionada =
            e.CurrentSelection
                .FirstOrDefault() as Serie;

        if (serieSeleccionada == null)
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

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}