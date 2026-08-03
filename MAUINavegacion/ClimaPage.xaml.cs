using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class ClimaPage : ContentPage
{
    private readonly WeatherService _weatherService;

    public ClimaPage()
    {
        InitializeComponent();

        _weatherService = new WeatherService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            Cargando.IsVisible = true;
            Cargando.IsRunning = true;

            RespuestaClima? clima =
                await _weatherService.ObtenerClimaActualAsync();

            if (clima == null)
                return;

            CiudadLabel.Text = clima.Ciudad;

            TemperaturaLabel.Text =
                $"{clima.Main.Temperatura:0}°C";

            DescripcionLabel.Text =
                clima.Weather[0].Descripcion;

            SensacionLabel.Text =
                $"Sensación térmica: {clima.Main.SensacionTermica:0}°C";

            HumedadLabel.Text =
                $"Humedad: {clima.Main.Humedad}%";

            IconoImage.Source =
                $"https://openweathermap.org/img/wn/{clima.Weather[0].Icono}@2x.png";

            List<PronosticoDia> pronostico =
    await _weatherService.ObtenerPronostico5DiasAsync();

            ListaPronostico.ItemsSource = pronostico;
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

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
