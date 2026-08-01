using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class SeriesPage : ContentPage
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
            return;

        try
        {
            Cargando.IsVisible = true;
            Cargando.IsRunning = true;
            ListaSeries.IsVisible = false;

            List<Serie> series =
                await _serieService.ObtenerSeriesAsync();

            ListaSeries.ItemsSource = series;
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

    private async void OnSerieSeleccionada(
        object sender,
        SelectionChangedEventArgs e)
    {
        Serie? serieSeleccionada =
            e.CurrentSelection.FirstOrDefault() as Serie;

        if (serieSeleccionada == null)
            return;

        ((CollectionView)sender).SelectedItem = null;

        await Navigation.PushAsync(
            new DetalleSeriePage(serieSeleccionada));
    }
    private async void OnVolverClicked(
    object sender,
    EventArgs e)
    {
        await Navigation.PopAsync();
    }
}