using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace MAUINavegacion;

public partial class LoginPage : ContentPage
{
    private bool _autenticando;

    public LoginPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        bool usuarioYaIngreso =
            Preferences.Default.Get(
                "UsuarioYaIngreso",
                false);

        if (usuarioYaIngreso)
        {
            await AutenticarConHuellaAsync();
        }
    }

    private async void OnHuellaClicked(
        object sender,
        EventArgs e)
    {
        await AutenticarConHuellaAsync();
    }

    private async Task AutenticarConHuellaAsync()
    {
        if (_autenticando)
        {
            return;
        }

        _autenticando = true;

        LimpiarErroresLogin();
        MensajeLabel.IsVisible = false;

        try
        {
            bool disponible =
                await CrossFingerprint.Current
                    .IsAvailableAsync(true);

            if (!disponible)
            {
                MostrarMensaje(
                    "La huella o biometría no está disponible en este dispositivo.");

                return;
            }

            var solicitud =
                new AuthenticationRequestConfiguration(
                    "Ingresar a RedFlix",
                    "Confirmá tu identidad con la huella digital")
                {
                    CancelTitle = "Cancelar",
                    FallbackTitle = "Usar contraseña"
                };

            FingerprintAuthenticationResult resultado =
                await CrossFingerprint.Current
                    .AuthenticateAsync(solicitud);

            if (resultado.Authenticated)
            {
                EntrarAlaSeleccionDePerfiles();
                return;
            }

            MostrarMensaje(
                resultado.Status ==
                FingerprintAuthenticationResultStatus.Canceled
                    ? "Autenticación cancelada."
                    : "No se pudo verificar la identidad.");
        }
        catch (Exception)
        {
          
               
        }
        finally
        {
            _autenticando = false;
        }
    }

    private async void OnIniciarSesionClicked(
        object sender,
        EventArgs e)
    {
        LimpiarErroresLogin();

        string nombreUsuario =
            UsuarioEntry.Text?.Trim() ??
            string.Empty;

        string contrasena =
            ContrasenaEntry.Text ??
            string.Empty;

        bool hayErrores = false;

        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            UsuarioErrorLabel.Text =
                "Ingresá tu nombre de usuario.";

            UsuarioErrorLabel.IsVisible = true;
            hayErrores = true;
        }

        if (string.IsNullOrWhiteSpace(contrasena))
        {
            ContrasenaErrorLabel.Text =
                "Ingresá tu contraseña.";

            ContrasenaErrorLabel.IsVisible = true;
            hayErrores = true;
        }

        if (hayErrores)
        {
            return;
        }

        try
        {
            await App.Database
                .InicializarAsync();

            var usuario =
                await App.Database
                    .ValidarLoginAsync(
                        nombreUsuario,
                        contrasena);

            if (usuario == null)
            {
                ContrasenaErrorLabel.Text =
                    "El usuario o la contraseña son incorrectos.";

                ContrasenaErrorLabel.IsVisible =
                    true;

                return;
            }

            Preferences.Default.Set(
                "UsuarioYaIngreso",
                true);

            Preferences.Default.Set(
                "UsuarioId",
                usuario.Id);

            Preferences.Default.Set(
                "NombreCompleto",
                usuario.NombreCompleto);

            Preferences.Default.Remove(
                "PerfilActivoId");

            Preferences.Default.Remove(
                "PerfilActivoNombre");

            EntrarAlaSeleccionDePerfiles();
        }
        catch (Exception)
        {
            ContrasenaErrorLabel.Text =
                "No se pudo iniciar sesión. Intentá nuevamente.";

            ContrasenaErrorLabel.IsVisible =
                true;
        }
    }

    private void LimpiarErroresLogin()
    {
        UsuarioErrorLabel.Text =
            string.Empty;

        UsuarioErrorLabel.IsVisible =
            false;

        ContrasenaErrorLabel.Text =
            string.Empty;

        ContrasenaErrorLabel.IsVisible =
            false;

        MensajeLabel.Text =
            string.Empty;

        MensajeLabel.IsVisible =
            false;
    }

    private void EntrarAlaSeleccionDePerfiles()
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new NavigationPage(
                    new SeleccionPerfilPage())
                {
                    BarBackgroundColor =
                        Color.FromArgb("#151515"),

                    BarTextColor =
                        Colors.White
                };
        }
    }

    private void MostrarMensaje(
        string mensaje)
    {
        MensajeLabel.Text =
            mensaje;

        MensajeLabel.IsVisible =
            true;
    }

    private async void OnCrearCuentaClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new RegistroPage());
    }
}