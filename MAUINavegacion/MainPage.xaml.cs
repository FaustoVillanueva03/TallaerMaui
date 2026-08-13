using MAUINavegacion.Models;

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

        bool esMenor =
            Preferences.Default.Get(
                "EsMenor18",
                false);

        PerfilActivoLabel.Text =
            esMenor
                ? $"👶 Perfil activo: {nombrePerfil}"
                : $"Perfil activo: {nombrePerfil}";

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
#if WINDOWS
        await DisplayAlert(
            "Mapa no disponible",
            "El mapa está disponible únicamente en la versión móvil.",
            "Aceptar");

        return;
#else
    try
    {
        await Navigation.PushAsync(
            new MapaPage());
    }
    catch (Exception ex)
    {
        await DisplayAlert(
            "Error",
            ex.Message,
            "Aceptar");
    }
#endif
    }

    private async void OnPerfilClicked(
        object sender,
        EventArgs e)
    {
        bool puedeEntrar =
            await VerificarAccesoPerfilesAsync();

        if (!puedeEntrar)
        {
            return;
        }

        await Navigation.PushAsync(
            new PerfilPage());
    }

    private async Task<bool>
        VerificarAccesoPerfilesAsync()
    {
        bool esMenor =
            Preferences.Default.Get(
                "EsMenor18",
                false);

        // Si no es un perfil infantil,
        // entra normalmente.
        if (!esMenor)
        {
            return true;
        }

        int usuarioId =
            Preferences.Default.Get(
                "UsuarioId",
                0);

        if (usuarioId == 0)
        {
            await DisplayAlert(
                "Control parental",
                "No se pudo identificar al usuario principal.",
                "Aceptar");

            return false;
        }

        string? contrasena =
            await DisplayPromptAsync(
                "🔒 Control parental",
                "Ingresá la contraseña del usuario principal para salir del perfil infantil.",
                "Confirmar",
                "Cancelar",
                "Contraseña",
                maxLength: 100,
                keyboard: Keyboard.Text);

        if (contrasena == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(
                contrasena))
        {
            await DisplayAlert(
                "Control parental",
                "Ingresá la contraseña.",
                "Aceptar");

            return false;
        }

        try
        {
            Usuario? usuario =
                await App.Database
                    .ObtenerUsuarioPorIdAsync(
                        usuarioId);

            if (usuario == null)
            {
                await DisplayAlert(
                    "Control parental",
                    "No se encontró el usuario principal.",
                    "Aceptar");

                return false;
            }

            if (usuario.Contrasena != contrasena)
            {
                await DisplayAlert(
                    "Contraseña incorrecta",
                    "No podés salir del perfil infantil.",
                    "Aceptar");

                return false;
            }

            return true;
        }
        catch (Exception error)
        {
            await DisplayAlert(
                "Error",
                $"No se pudo validar la contraseña.\n\n{error.Message}",
                "Aceptar");

            return false;
        }
    }
}