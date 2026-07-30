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
            Preferences.Default.Get("UsuarioYaIngreso", false);

        if (usuarioYaIngreso)
        {
            await AutenticarConHuellaAsync();
        }
    }

    private async void OnHuellaClicked(object sender, EventArgs e)
    {
        await AutenticarConHuellaAsync();
    }

    private async Task AutenticarConHuellaAsync()
    {
        if (_autenticando)
            return;

        _autenticando = true;
        MensajeLabel.IsVisible = false;

        try
        {
            bool disponible =
                await CrossFingerprint.Current.IsAvailableAsync(true);

            if (!disponible)
            {
                MostrarMensaje(
                    "La huella o biometría no está disponible en este dispositivo.");

                return;
            }

            var solicitud = new AuthenticationRequestConfiguration(
                "Ingresar a RedFlix",
                "Confirmá tu identidad con la huella digital")
            {
                CancelTitle = "Cancelar",
                FallbackTitle = "Usar contraseña"
            };

            FingerprintAuthenticationResult resultado =
                await CrossFingerprint.Current.AuthenticateAsync(solicitud);

            if (resultado.Authenticated)
            {
                await EntrarAlaAplicacionAsync();
                return;
            }

            MostrarMensaje(
                resultado.Status == FingerprintAuthenticationResultStatus.Canceled
                    ? "Autenticación cancelada."
                    : "No se pudo verificar la identidad.");
        }
        catch (Exception)
        {
            MostrarMensaje(
                "La autenticación con huella no está disponible en esta plataforma.");
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
        string usuario = UsuarioEntry.Text?.Trim() ?? "";
        string contrasena = ContrasenaEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(usuario) ||
            string.IsNullOrWhiteSpace(contrasena))
        {
            MostrarMensaje(
                "Ingresá el usuario y la contraseña.");

            return;
        }

        /*
         * Validación temporal.
         * Después se reemplaza por la consulta a SQLite.
         */
        if (usuario != "admin" || contrasena != "1234")
        {
            MostrarMensaje(
                "Usuario o contraseña incorrectos.");

            return;
        }

        Preferences.Default.Set("UsuarioYaIngreso", true);

        await EntrarAlaAplicacionAsync();
    }

    private async Task EntrarAlaAplicacionAsync()
    {
        Preferences.Default.Set("UsuarioYaIngreso", true);

        Application.Current!.MainPage =
            new AppShell();

        await Task.CompletedTask;
    }

    private void MostrarMensaje(string mensaje)
    {
        MensajeLabel.Text = mensaje;
        MensajeLabel.IsVisible = true;
    }
}