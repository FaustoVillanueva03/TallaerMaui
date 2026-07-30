namespace MAUINavegacion;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCineClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PeliculasPage());
    }

    private async void OnSeriesClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Series",
            "La sección de series todavía no está creada.",
            "Aceptar");
    }

    private async void OnClimaClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Clima",
            "La sección de clima todavía no está creada.",
            "Aceptar");
    }

    private async void OnCotizacionesClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Cotizaciones",
            "La sección de cotizaciones todavía no está creada.",
            "Aceptar");
    }

    private async void OnMapaClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Mapa",
            "La sección de mapa todavía no está creada.",
            "Aceptar");
    }

    private async void OnPerfilClicked(object sender, EventArgs e)
    {
        await DisplayAlert(
            "Perfil",
            "La sección de perfil todavía no está creada.",
            "Aceptar");
    }
}