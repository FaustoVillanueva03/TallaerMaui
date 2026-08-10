using MAUINavegacion.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MAUINavegacion;

public partial class MapaPage : ContentPage
{
    public MapaPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CargarMapaAsync();
    }

    private async Task CargarMapaAsync()
    {
        try
        {
            Mapa.Pins.Clear();

            int usuarioId =
                Preferences.Default.Get(
                    "UsuarioId",
                    0);

            if (usuarioId == 0)
            {
                await DisplayAlert(
                    "Sesión",
                    "No se encontró el usuario que inició sesión.",
                    "Aceptar");

                return;
            }

            List<Perfil> perfiles =
                await App.Database
                    .ObtenerPerfilesAsync(usuarioId);

            AgregarPinsDePerfiles(perfiles);

            await MostrarUbicacionActualAsync();
        }
        catch (Exception error)
        {
            await DisplayAlert(
                "Mapa",
                $"No se pudo cargar el mapa.\n{error.Message}",
                "Aceptar");
        }
    }

    private void AgregarPinsDePerfiles(
        List<Perfil> perfiles)
    {
        foreach (Perfil perfil in perfiles)
        {
            if (perfil.Latitud == 0 &&
                perfil.Longitud == 0)
            {
                continue;
            }

            Location ubicacion =
                new Location(
                    perfil.Latitud,
                    perfil.Longitud);

            Pin pin =
                new Pin
                {
                    Label =
                        $"Perfil: {perfil.Nombre}",

                    Address =
                        string.IsNullOrWhiteSpace(
                            perfil.Direccion)
                            ? "Ubicación del perfil"
                            : perfil.Direccion,

                    Location = ubicacion,

                    Type = PinType.Place
                };

            Mapa.Pins.Add(pin);
        }
    }

    private async Task MostrarUbicacionActualAsync()
    {
        try
        {
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
                return;
            }

            Mapa.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    ubicacion,
                    Distance.FromKilometers(5)));
        }
        catch (Exception)
        {
            // Si no se puede obtener la ubicación
            // el mapa igualmente muestra los perfiles.
        }
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}