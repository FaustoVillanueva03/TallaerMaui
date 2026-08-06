using MAUINavegacion.Models;

namespace MAUINavegacion;

public partial class SeleccionPerfilPage : ContentPage
{
    public SeleccionPerfilPage()
    {
        InitializeComponent();

        string nombreCompleto =
            Preferences.Default.Get(
                "NombreCompleto",
                string.Empty);

        NombreUsuarioLabel.Text =
            string.IsNullOrWhiteSpace(nombreCompleto)
                ? "Elegí un perfil para continuar"
                : $"Cuenta de {nombreCompleto}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CargarPerfilesAsync();
    }

    private async Task CargarPerfilesAsync()
    {
        try
        {
            MostrarCargando(true);

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

                CerrarSesion();

                return;
            }

            List<Perfil> perfiles =
                await App.Database
                    .ObtenerPerfilesAsync(usuarioId);

            // Si la cuenta es anterior y todavía no tiene perfiles,
            // se crea automáticamente el perfil principal.
            if (perfiles.Count == 0)
            {
                Usuario? usuario =
                    await App.Database
                        .ObtenerUsuarioPorIdAsync(usuarioId);

                if (usuario != null)
                {
                    Perfil perfilPrincipal =
                        new()
                        {
                            UsuarioId =
                                usuario.Id,

                            Nombre =
                                usuario.NombreCompleto,

                            Direccion =
                                usuario.Direccion,

                            Telefono =
                                usuario.Telefono,

                            Email =
                                usuario.Email,

                            RutaFoto =
                                usuario.RutaFoto,

                            Latitud =
                                0,

                            Longitud =
                                0,

                            MostrarPeliculas =
                                true,

                            MostrarSeries =
                                true,

                            MostrarClima =
                                true,

                            MostrarCotizaciones =
                                true,

                            MostrarMapa =
                                true
                        };

                    await App.Database
                        .CrearPerfilAsync(
                            perfilPrincipal);

                    perfiles =
                        await App.Database
                            .ObtenerPerfilesAsync(usuarioId);
                }
            }

            ListaPerfiles.ItemsSource = null;
            ListaPerfiles.ItemsSource = perfiles;

            bool sinPerfiles =
                perfiles.Count == 0;

            ListaPerfiles.IsVisible =
                !sinPerfiles;

            SinPerfilesLayout.IsVisible =
                sinPerfiles;
        }
        catch (Exception error)
        {
            await DisplayAlert(
                "Error",
                $"No se pudieron cargar los perfiles.\n{error.Message}",
                "Aceptar");
        }
        finally
        {
            MostrarCargando(false);
        }
    }

    private void OnElegirPerfilClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button boton ||
            boton.BindingContext is not Perfil perfil)
        {
            return;
        }

        Preferences.Default.Set(
            "PerfilActivoId",
            perfil.Id);

        Preferences.Default.Set(
            "PerfilActivoNombre",
            perfil.Nombre);

        Preferences.Default.Set(
            "MostrarPeliculas",
            perfil.MostrarPeliculas);

        Preferences.Default.Set(
            "MostrarSeries",
            perfil.MostrarSeries);

        Preferences.Default.Set(
            "MostrarClima",
            perfil.MostrarClima);

        Preferences.Default.Set(
            "MostrarCotizaciones",
            perfil.MostrarCotizaciones);

        Preferences.Default.Set(
            "MostrarMapa",
            perfil.MostrarMapa);

        AbrirPaginaPrincipal();
    }

    private async void OnAdministrarPerfilesClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new PerfilPage());
    }

    private async void OnCerrarSesionClicked(
        object sender,
        EventArgs e)
    {
        bool confirmar =
            await DisplayAlert(
                "Cerrar sesión",
                "¿Querés cerrar la sesión actual?",
                "Cerrar sesión",
                "Cancelar");

        if (!confirmar)
        {
            return;
        }

        CerrarSesion();
    }

    private void AbrirPaginaPrincipal()
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new NavigationPage(
                    new MainPage())
                {
                    BarBackgroundColor =
                        Color.FromArgb("#151515"),

                    BarTextColor =
                        Colors.White
                };
        }
    }

    private void CerrarSesion()
    {
        Preferences.Default.Remove(
            "UsuarioYaIngreso");

        Preferences.Default.Remove(
            "UsuarioId");

        Preferences.Default.Remove(
            "NombreCompleto");

        Preferences.Default.Remove(
            "PerfilActivoId");

        Preferences.Default.Remove(
            "PerfilActivoNombre");

        Preferences.Default.Remove(
            "MostrarPeliculas");

        Preferences.Default.Remove(
            "MostrarSeries");

        Preferences.Default.Remove(
            "MostrarClima");

        Preferences.Default.Remove(
            "MostrarCotizaciones");

        Preferences.Default.Remove(
            "MostrarMapa");

        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new NavigationPage(
                    new LoginPage())
                {
                    BarBackgroundColor =
                        Color.FromArgb("#151515"),

                    BarTextColor =
                        Colors.White
                };
        }
    }

    private void MostrarCargando(
        bool mostrar)
    {
        CargandoLayout.IsVisible =
            mostrar;

        CargandoIndicator.IsRunning =
            mostrar;
    }
}