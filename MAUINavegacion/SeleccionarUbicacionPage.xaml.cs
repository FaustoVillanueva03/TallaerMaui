using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MAUINavegacion;

public partial class SeleccionarUbicacionPage : ContentPage
{
    private Location? _ubicacionSeleccionada;

    public double LatitudSeleccionada { get; private set; }

    public double LongitudSeleccionada { get; private set; }

    public bool UbicacionGuardada { get; private set; }

    public SeleccionarUbicacionPage(
        double latitudInicial = 0,
        double longitudInicial = 0)
    {
        InitializeComponent();

        MostrarMapaInicial(
            latitudInicial,
            longitudInicial);
    }

    private void MostrarMapaInicial(
        double latitud,
        double longitud)
    {
        Location ubicacion;

        if (latitud != 0 && longitud != 0)
        {
            ubicacion =
                new Location(
                    latitud,
                    longitud);
        }
        else
        {
            // Punta del Este
            ubicacion =
                new Location(
                    -34.9475,
                    -54.9338);
        }

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

        LatitudSeleccionada =
            e.Location.Latitude;

        LongitudSeleccionada =
            e.Location.Longitude;

        Mapa.Pins.Clear();

        Pin pin =
            new Pin
            {
                Label = "Ubicación del perfil",
                Location = e.Location,
                Type = PinType.Place
            };

        Mapa.Pins.Add(pin);

        UbicacionLabel.Text =
            $"Ubicación seleccionada\n" +
            $"Latitud: {LatitudSeleccionada:F6}\n" +
            $"Longitud: {LongitudSeleccionada:F6}";

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

        UbicacionGuardada = true;

        await Navigation.PopAsync();
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}