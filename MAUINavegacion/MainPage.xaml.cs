namespace MAUINavegacion;

public partial class MainPage : BasePage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        AplicarPreferenciasDelPerfil();
    }

    private void AplicarPreferenciasDelPerfil()
    {
        string nombrePerfil =
            Preferences.Default.Get(
                "PerfilActivoNombre",
                "Perfil principal");

        PerfilActivoLabel.Text =
            $"Perfil activo: {nombrePerfil}";

        bool mostrarPeliculas =
            Preferences.Default.Get(
                "MostrarPeliculas",
                true);

        bool mostrarSeries =
            Preferences.Default.Get(
                "MostrarSeries",
                true);

        bool mostrarClima =
            Preferences.Default.Get(
                "MostrarClima",
                true);

        bool mostrarCotizaciones =
            Preferences.Default.Get(
                "MostrarCotizaciones",
                true);

        bool mostrarMapa =
            Preferences.Default.Get(
                "MostrarMapa",
                true);

        PeliculasCard.IsVisible =
            mostrarPeliculas;

        SeriesCard.IsVisible =
            mostrarSeries;

        ClimaCard.IsVisible =
            mostrarClima;

        CotizacionesCard.IsVisible =
            mostrarCotizaciones;

        MapaCard.IsVisible =
            mostrarMapa;

        AjustarClimaYCotizaciones(
            mostrarClima,
            mostrarCotizaciones);

        AjustarMapaYPerfil(
            mostrarMapa);
    }

    private void AjustarClimaYCotizaciones(
        bool mostrarClima,
        bool mostrarCotizaciones)
    {
        ClimaCotizacionesGrid.IsVisible =
            mostrarClima ||
            mostrarCotizaciones;

        if (mostrarClima &&
            mostrarCotizaciones)
        {
            Grid.SetColumn(
                ClimaCard,
                0);

            Grid.SetColumnSpan(
                ClimaCard,
                1);

            Grid.SetColumn(
                CotizacionesCard,
                1);

            Grid.SetColumnSpan(
                CotizacionesCard,
                1);

            return;
        }

        if (mostrarClima)
        {
            Grid.SetColumn(
                ClimaCard,
                0);

            Grid.SetColumnSpan(
                ClimaCard,
                2);
        }

        if (mostrarCotizaciones)
        {
            Grid.SetColumn(
                CotizacionesCard,
                0);

            Grid.SetColumnSpan(
                CotizacionesCard,
                2);
        }
    }

    private void AjustarMapaYPerfil(
        bool mostrarMapa)
    {
        PerfilCard.IsVisible = true;

        if (mostrarMapa)
        {
            Grid.SetColumn(
                MapaCard,
                0);

            Grid.SetColumnSpan(
                MapaCard,
                1);

            Grid.SetColumn(
                PerfilCard,
                1);

            Grid.SetColumnSpan(
                PerfilCard,
                1);
        }
        else
        {
            Grid.SetColumn(
                PerfilCard,
                0);

            Grid.SetColumnSpan(
                PerfilCard,
                2);
        }
    }

    private async void OnCineClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new PeliculasPage());
    }

    private async void OnSeriesClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new SeriesPage());
    }

    private async void OnClimaClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new ClimaPage());
    }

    private async void OnCotizacionesClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new CotizacionesPage());
    }

    private async void OnMapaClicked(
        object sender,
        EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(
                new MapaPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                ex.ToString(),
                "Aceptar");
        }
    }

    private async void OnPerfilClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new PerfilPage());
    }
}