using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MAUINavegacion;

public partial class SeleccionarUbicacionPage : ContentPage
{
    private Location? _ubicacionSeleccionada;

    private readonly Action<double, double> _alGuardarUbicacion;

    private readonly double _latitudInicial;
    private readonly double _longitudInicial;

    public SeleccionarUbicacionPage(
        double latitudInicial,
        double longitudInicial,
        Action<double, double> alGuardarUbicacion)
    {
        InitializeComponent();

        _latitudInicial = latitudInicial;
        _longitudInicial = longitudInicial;

        _alGuardarUbicacion = alGuardarUbicacion;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_latitudInicial != 0 ||
            _longitudInicial != 0)
        {
            MostrarUbicacionExistente(
                _latitudInicial,
                _longitudInicial);

            return;
        }

        await MostrarUbicacionActualAsync();
    }

    private void MostrarUbicacionExistente(
        double latitud,
        double longitud)
    {
        Location ubicacion =
            new Location(
                latitud,
                longitud);

        _ubicacionSeleccionada =
            ubicacion;

        Mapa.Pins.Clear();

        Pin pin =
            new Pin
            {
                Label = "Ubicación del perfil",
                Address = "Ubicación guardada",
                Location = ubicacion,
                Type = PinType.Place
            };

        Mapa.Pins.Add(pin);

        Mapa.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                ubicacion,
                Distance.FromKilometers(2)));

        UbicacionLabel.Text =
            $"Ubicación actual del perfil\n" +
            $"Latitud: {latitud:F6}\n" +
            $"Longitud: {longitud:F6}";

        GuardarUbicacionButton.IsEnabled = true;
    }

    private async Task MostrarUbicacionActualAsync()
    {
        try
        {
            PermissionStatus permiso =
                await Permissions.RequestAsync<
                    Permissions.LocationWhenInUse>();

            if (permiso != PermissionStatus.Granted)
            {
                MostrarPuntaDelEste();
                return;
            }

            Location? ubicacion =
                await Geolocation.Default
                    .GetLocationAsync(
                        new GeolocationRequest
                        {
                            DesiredAccuracy =
                                GeolocationAccuracy.Medium,

                            Timeout =
                                TimeSpan.FromSeconds(10)
                        });

            if (ubicacion == null)
            {
                MostrarPuntaDelEste();
                return;
            }

            Mapa.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    ubicacion,
                    Distance.FromKilometers(3)));
        }
        catch
        {
            MostrarPuntaDelEste();
        }
    }

    private void MostrarPuntaDelEste()
    {
        Location ubicacion =
            new Location(
                -34.9475,
                -54.9338);

        Mapa.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                ubicacion,
                Distance.FromKilometers(5)));
    }

    private void OnMapaClicked(
        object sender,
        MapClickedEventArgs e)
    {
        _ubicacionSeleccionada =
            e.Location;

        Mapa.Pins.Clear();

        Pin pin =
            new Pin
            {
                Label = "Ubicación del perfil",
                Address = "Punto seleccionado",
                Location = e.Location,
                Type = PinType.Place
            };

        Mapa.Pins.Add(pin);

        UbicacionLabel.Text =
            $"Ubicación seleccionada\n" +
            $"Latitud: {e.Location.Latitude:F6}\n" +
            $"Longitud: {e.Location.Longitude:F6}";

        GuardarUbicacionButton.IsEnabled = true;
    }

    private async void OnGuardarUbicacionClicked(
        object sender,
        EventArgs e)
    {
        if (_ubicacionSeleccionada == null)
        {
            await DisplayAlert(
                "Ubicación",
                "Primero tocá un punto en el mapa.",
                "Aceptar");

            return;
        }

        _alGuardarUbicacion(
            _ubicacionSeleccionada.Latitude,
            _ubicacionSeleccionada.Longitude);

        await Navigation.PopAsync();
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}