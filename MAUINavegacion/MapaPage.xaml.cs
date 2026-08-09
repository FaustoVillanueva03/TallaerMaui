using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MAUINavegacion;

public partial class MapaPage : ContentPage
{
    public MapaPage()
    {
        InitializeComponent();

        MostrarMapa();
    }

    private void MostrarMapa()
    {
        Location puntaDelEste =
            new Location(
                -34.9475,
                -54.9338);

        Mapa.MoveToRegion(
            MapSpan.FromCenterAndRadius(
                puntaDelEste,
                Distance.FromKilometers(5)));
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}