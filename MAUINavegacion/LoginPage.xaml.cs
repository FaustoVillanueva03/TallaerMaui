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
            return;

        _autenticando = true;
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
                EntrarAlaAplicacion();
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
        MensajeLabel.IsVisible = false;

        string nombreUsuario =
            UsuarioEntry.Text?.Trim() ?? string.Empty;

        string contrasena =
            ContrasenaEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrWhiteSpace(contrasena))
        {
            MostrarMensaje(
                "Ingresá el usuario y la contraseña.");

            return;
        }

        try
        {
            await App.Database.InicializarAsync();

            var usuario =
                await App.Database.ValidarLoginAsync(
                    nombreUsuario,
                    contrasena);

            if (usuario == null)
            {
                MostrarMensaje(
                    "Usuario o contraseña incorrectos.");

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

            await DisplayAlert(
                "Bienvenido",
                $"Hola, {usuario.NombreCompleto}",
                "Aceptar");

            EntrarAlaAplicacion();
        }
        catch (Exception error)
        {
            MostrarMensaje(
                $"No se pudo iniciar sesión: {error.Message}");
        }
    }

    private void EntrarAlaAplicacion()
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page =
                new NavigationPage(new MainPage())
                {
                    BarBackgroundColor =
                        Color.FromArgb("#151515"),

                    BarTextColor = Colors.White
                };
        }
    }

    private void MostrarMensaje(string mensaje)
    {
        MensajeLabel.Text = mensaje;
        MensajeLabel.IsVisible = true;
    }

    private async void OnCrearCuentaClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new RegistroPage());
    }
}