using MAUINavegacion.Models;

namespace MAUINavegacion;

public partial class PerfilPage : BasePage
{
    private Perfil? _perfilEditando;
    private string _rutaFotoSeleccionada = string.Empty;

    public PerfilPage()
    {
        InitializeComponent();
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

                ListaPerfiles.ItemsSource =
                    new List<Perfil>();

                SinPerfilesLayout.IsVisible = true;

                return;
            }

            List<Perfil> perfiles =
                await App.Database
                    .ObtenerPerfilesAsync(usuarioId);

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

    private void OnNuevoPerfilClicked(
        object sender,
        EventArgs e)
    {
        _perfilEditando = null;

        LimpiarFormulario();

        TituloFormularioLabel.Text =
            "Nuevo perfil";

        GuardarPerfilButton.Text =
            "Crear perfil";

        FormularioFondo.IsVisible = true;
    }

    private void OnEditarPerfilClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button boton ||
            boton.BindingContext is not Perfil perfil)
        {
            return;
        }

        _perfilEditando = perfil;

        TituloFormularioLabel.Text =
            "Editar perfil";

        GuardarPerfilButton.Text =
            "Guardar cambios";

        NombreEntry.Text =
            perfil.Nombre;

        EmailEntry.Text =
            perfil.Email;

        TelefonoEntry.Text =
            perfil.Telefono;

        DireccionEntry.Text =
            perfil.Direccion;

        MostrarPeliculasSwitch.IsToggled =
            perfil.MostrarPeliculas;

        MostrarSeriesSwitch.IsToggled =
            perfil.MostrarSeries;

        MostrarClimaSwitch.IsToggled =
            perfil.MostrarClima;

        MostrarCotizacionesSwitch.IsToggled =
            perfil.MostrarCotizaciones;

        MostrarMapaSwitch.IsToggled =
            perfil.MostrarMapa;

        _rutaFotoSeleccionada =
            perfil.RutaFoto;

        MostrarFotoSeleccionada();

        NombreErrorLabel.IsVisible = false;
        FormularioFondo.IsVisible = true;
    }

    private async void OnGuardarPerfilClicked(
        object sender,
        EventArgs e)
    {
        NombreErrorLabel.IsVisible = false;

        string nombre =
            NombreEntry.Text?.Trim() ??
            string.Empty;

        if (string.IsNullOrWhiteSpace(nombre))
        {
            NombreErrorLabel.Text =
                "Ingresá un nombre para el perfil.";

            NombreErrorLabel.IsVisible = true;

            return;
        }

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

        try
        {
            GuardarPerfilButton.IsEnabled = false;
            GuardarPerfilButton.Text = "Guardando...";

            if (_perfilEditando == null)
            {
                Perfil nuevoPerfil =
                    new()
                    {
                        UsuarioId = usuarioId,

                        Nombre = nombre,

                        Email =
                            EmailEntry.Text?.Trim() ??
                            string.Empty,

                        Telefono =
                            TelefonoEntry.Text?.Trim() ??
                            string.Empty,

                        Direccion =
                            DireccionEntry.Text?.Trim() ??
                            string.Empty,

                        RutaFoto =
                            _rutaFotoSeleccionada,

                        MostrarPeliculas =
                            MostrarPeliculasSwitch.IsToggled,

                        MostrarSeries =
                            MostrarSeriesSwitch.IsToggled,

                        MostrarClima =
                            MostrarClimaSwitch.IsToggled,

                        MostrarCotizaciones =
                            MostrarCotizacionesSwitch.IsToggled,

                        MostrarMapa =
                            MostrarMapaSwitch.IsToggled
                    };

                await App.Database
                    .CrearPerfilAsync(nuevoPerfil);
            }
            else
            {
                _perfilEditando.Nombre =
                    nombre;

                _perfilEditando.Email =
                    EmailEntry.Text?.Trim() ??
                    string.Empty;

                _perfilEditando.Telefono =
                    TelefonoEntry.Text?.Trim() ??
                    string.Empty;

                _perfilEditando.Direccion =
                    DireccionEntry.Text?.Trim() ??
                    string.Empty;

                _perfilEditando.RutaFoto =
                    _rutaFotoSeleccionada;

                _perfilEditando.MostrarPeliculas =
                    MostrarPeliculasSwitch.IsToggled;

                _perfilEditando.MostrarSeries =
                    MostrarSeriesSwitch.IsToggled;

                _perfilEditando.MostrarClima =
                    MostrarClimaSwitch.IsToggled;

                _perfilEditando.MostrarCotizaciones =
                    MostrarCotizacionesSwitch.IsToggled;

                _perfilEditando.MostrarMapa =
                    MostrarMapaSwitch.IsToggled;

                await App.Database
                    .ActualizarPerfilAsync(
                        _perfilEditando);

                ActualizarPreferenciasSiEsPerfilActivo(
                    _perfilEditando);
            }

            FormularioFondo.IsVisible = false;

            await CargarPerfilesAsync();
        }
        catch (Exception error)
        {
            await DisplayAlert(
                "Error",
                $"No se pudo guardar el perfil.\n{error.Message}",
                "Aceptar");
        }
        finally
        {
            GuardarPerfilButton.IsEnabled = true;

            GuardarPerfilButton.Text =
                _perfilEditando == null
                    ? "Crear perfil"
                    : "Guardar cambios";
        }
    }

    private void ActualizarPreferenciasSiEsPerfilActivo(
        Perfil perfil)
    {
        int perfilActivoId =
            Preferences.Default.Get(
                "PerfilActivoId",
                0);

        if (perfilActivoId != perfil.Id)
        {
            return;
        }

        GuardarPerfilActivoEnPreferences(
            perfil);
    }

    private async void OnEliminarPerfilClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button boton ||
            boton.BindingContext is not Perfil perfil)
        {
            return;
        }

        bool confirmar =
            await DisplayAlert(
                "Eliminar perfil",
                $"¿Querés eliminar el perfil \"{perfil.Nombre}\"?",
                "Eliminar",
                "Cancelar");

        if (!confirmar)
        {
            return;
        }

        try
        {
            await App.Database
                .EliminarPerfilAsync(perfil);

            int perfilActivoId =
                Preferences.Default.Get(
                    "PerfilActivoId",
                    0);

            if (perfilActivoId == perfil.Id)
            {
                LimpiarPerfilActivo();
            }

            await CargarPerfilesAsync();
        }
        catch (Exception error)
        {
            await DisplayAlert(
                "Error",
                $"No se pudo eliminar el perfil.\n{error.Message}",
                "Aceptar");
        }
    }

    private async void OnUsarPerfilClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button boton ||
            boton.BindingContext is not Perfil perfil)
        {
            return;
        }

        GuardarPerfilActivoEnPreferences(
            perfil);

        await DisplayAlert(
            "Perfil activo",
            $"Ahora estás usando el perfil \"{perfil.Nombre}\".",
            "Aceptar");

        AbrirPaginaPrincipal();
    }

    private static void GuardarPerfilActivoEnPreferences(
        Perfil perfil)
    {
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
    }

    private static void LimpiarPerfilActivo()
    {
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

    private async void OnElegirFotoClicked(
        object sender,
        EventArgs e)
    {
        try
        {
            FileResult? resultado =
                await MediaPicker.Default
                    .PickPhotoAsync();

            if (resultado == null)
            {
                return;
            }

            string carpeta =
                FileSystem.AppDataDirectory;

            string extension =
                Path.GetExtension(
                    resultado.FileName);

            string nombreArchivo =
                $"perfil_{Guid.NewGuid()}{extension}";

            string rutaDestino =
                Path.Combine(
                    carpeta,
                    nombreArchivo);

            await using Stream origen =
                await resultado.OpenReadAsync();

            await using FileStream destino =
                File.Create(rutaDestino);

            await origen.CopyToAsync(destino);

            _rutaFotoSeleccionada =
                rutaDestino;

            MostrarFotoSeleccionada();
        }
        catch (Exception)
        {
            await DisplayAlert(
                "Foto",
                "No se pudo seleccionar la imagen.",
                "Aceptar");
        }
    }

    private void MostrarFotoSeleccionada()
    {
        bool tieneFoto =
            !string.IsNullOrWhiteSpace(
                _rutaFotoSeleccionada) &&
            File.Exists(
                _rutaFotoSeleccionada);

        FotoPerfilImage.Source =
            tieneFoto
                ? ImageSource.FromFile(
                    _rutaFotoSeleccionada)
                : null;

        FotoPerfilImage.IsVisible =
            tieneFoto;

        FotoVaciaLabel.IsVisible =
            !tieneFoto;
    }

    private void LimpiarFormulario()
    {
        NombreEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        TelefonoEntry.Text = string.Empty;
        DireccionEntry.Text = string.Empty;

        NombreErrorLabel.Text =
            string.Empty;

        NombreErrorLabel.IsVisible =
            false;

        MostrarPeliculasSwitch.IsToggled =
            true;

        MostrarSeriesSwitch.IsToggled =
            true;

        MostrarClimaSwitch.IsToggled =
            true;

        MostrarCotizacionesSwitch.IsToggled =
            true;

        MostrarMapaSwitch.IsToggled =
            true;

        _rutaFotoSeleccionada =
            string.Empty;

        MostrarFotoSeleccionada();
    }

    private void OnCerrarFormularioClicked(
        object sender,
        EventArgs e)
    {
        FormularioFondo.IsVisible = false;

        _perfilEditando = null;
    }

    private void MostrarCargando(
        bool mostrar)
    {
        CargandoLayout.IsVisible =
            mostrar;

        CargandoIndicator.IsRunning =
            mostrar;
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}