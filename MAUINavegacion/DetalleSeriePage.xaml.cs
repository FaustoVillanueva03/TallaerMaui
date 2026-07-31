using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class DetalleSeriePage : ContentPage
{
    private readonly SerieService _serieService;
    private Serie? _serie;
    private bool _trailerCargado;

    public DetalleSeriePage()
    {
        InitializeComponent();

        _serieService = new SerieService();
    }

    public DetalleSeriePage(Serie serie) : this()
    {
        _serie = serie;
        BindingContext = serie;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_trailerCargado || _serie == null)
            return;

        _trailerCargado = true;

        await CargarTrailerAsync();
    }

    private async Task CargarTrailerAsync()
    {
        try
        {
            Trailer? trailer =
                await _serieService.ObtenerTrailerAsync(_serie!.Id);

            if (trailer == null ||
                string.IsNullOrWhiteSpace(trailer.Key))
            {
                MensajeSinTrailer.Text =
                    "No hay un tráiler disponible para esta serie.";

                MensajeSinTrailer.IsVisible = true;
                SeccionTrailer.IsVisible = false;

                return;
            }

            TrailerWebView.Source =
                $"https://www.youtube.com/embed/{trailer.Key}";

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