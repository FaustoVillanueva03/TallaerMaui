using System.Text.RegularExpressions;
using MAUINavegacion.Models;
using SQLite;

namespace MAUINavegacion;

public partial class RegistroPage : ContentPage
{
    private string _rutaFoto = string.Empty;

    public RegistroPage()
    {
        InitializeComponent();
    }

    private async void OnSeleccionarFotoClicked(
        object sender,
        EventArgs e)
    {
        LimpiarMensajeGeneral();

        try
        {
            FileResult? foto =
                await MediaPicker.Default.PickPhotoAsync();

            if (foto == null)
                return;

            string nombreArchivo =
                $"{Guid.NewGuid()}{Path.GetExtension(foto.FileName)}";

            string rutaDestino = Path.Combine(
                FileSystem.AppDataDirectory,
                nombreArchivo);

            await using Stream origen =
                await foto.OpenReadAsync();

            await using FileStream destino =
                File.Create(rutaDestino);

            await origen.CopyToAsync(destino);

            _rutaFoto = rutaDestino;

            FotoPerfilImage.Source =
                ImageSource.FromFile(_rutaFoto);

            FotoPerfilImage.IsVisible = true;
        }
        catch (Exception)
        {
            MostrarMensajeGeneral(
                "No se pudo seleccionar la foto.",
                esError: true);
        }
    }

    private async void OnRegistrarClicked(
        object sender,
        EventArgs e)
    {
        LimpiarErrores();

        string nombreUsuario =
            UsuarioEntry.Text?.Trim() ?? string.Empty;

        string contrasena =
            ContrasenaEntry.Text ?? string.Empty;

        string nombreCompleto =
            NombreCompletoEntry.Text?.Trim() ?? string.Empty;

        string direccion =
            DireccionEntry.Text?.Trim() ?? string.Empty;

        string telefono =
            TelefonoEntry.Text?.Trim() ?? string.Empty;

        string email =
            EmailEntry.Text?.Trim() ?? string.Empty;

        bool hayErrores = false;

        // Validación del usuario

        if (string.IsNullOrWhiteSpace(nombreUsuario))
        {
            MostrarError(
                UsuarioErrorLabel,
                "Ingresá un nombre de usuario.");

            hayErrores = true;
        }
        else if (nombreUsuario.Length < 3)
        {
            MostrarError(
                UsuarioErrorLabel,
                "El usuario debe tener al menos 3 caracteres.");

            hayErrores = true;
        }

        // Validación de la contraseña

        if (string.IsNullOrWhiteSpace(contrasena))
        {
            MostrarError(
                ContrasenaErrorLabel,
                "Ingresá una contraseña.");

            hayErrores = true;
        }
        else if (contrasena.Length < 4)
        {
            MostrarError(
                ContrasenaErrorLabel,
                "La contraseña debe tener al menos 4 caracteres.");

            hayErrores = true;
        }

        // Validación del nombre

        if (string.IsNullOrWhiteSpace(nombreCompleto))
        {
            MostrarError(
                NombreErrorLabel,
                "Ingresá tu nombre completo.");

            hayErrores = true;
        }

        // Validación de la dirección

        if (string.IsNullOrWhiteSpace(direccion))
        {
            MostrarError(
                DireccionErrorLabel,
                "Ingresá tu dirección.");

            hayErrores = true;
        }

        // Validación del teléfono

        if (string.IsNullOrWhiteSpace(telefono))
        {
            MostrarError(
                TelefonoErrorLabel,
                "Ingresá tu teléfono.");

            hayErrores = true;
        }
        else if (!Regex.IsMatch(
                     telefono,
                     @"^[0-9+\-\s]{6,20}$"))
        {
            MostrarError(
                TelefonoErrorLabel,
                "Ingresá un teléfono válido.");

            hayErrores = true;
        }

        // Validación del email

        if (string.IsNullOrWhiteSpace(email))
        {
            MostrarError(
                EmailErrorLabel,
                "Ingresá tu email.");

            hayErrores = true;
        }
        else if (!Regex.IsMatch(
                     email,
                     @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            MostrarError(
                EmailErrorLabel,
                "Ingresá un email válido.");

            hayErrores = true;
        }

        if (hayErrores)
            return;

        try
        {
            await App.Database.InicializarAsync();

            Usuario? usuarioExistente =
                await App.Database
                    .ObtenerUsuarioPorNombreAsync(
                        nombreUsuario);

            if (usuarioExistente != null)
            {
                MostrarError(
                    UsuarioErrorLabel,
                    "Ese nombre de usuario ya existe.");

                return;
            }

            Usuario nuevoUsuario = new()
            {
                NombreUsuario = nombreUsuario,
                Contrasena = contrasena,
                NombreCompleto = nombreCompleto,
                Direccion = direccion,
                Telefono = telefono,
                Email = email,
                RutaFoto = _rutaFoto
            };

            await App.Database
                .RegistrarUsuarioAsync(nuevoUsuario);

            MostrarMensajeGeneral(
                "Usuario registrado correctamente.",
                esError: false);

            await Task.Delay(1200);

            await Navigation.PopAsync();
        }
        catch (SQLiteException error)
        {
            if (error.Message.Contains(
                    "UNIQUE",
                    StringComparison.OrdinalIgnoreCase))
            {
                MostrarError(
                    EmailErrorLabel,
                    "El usuario o el email ya están registrados.");
            }
            else
            {
                MostrarMensajeGeneral(
                    "No se pudo guardar el usuario.",
                    esError: true);
            }
        }
        catch (Exception)
        {
            MostrarMensajeGeneral(
                "Ocurrió un error al registrar el usuario.",
                esError: true);
        }
    }

    private void MostrarError(
        Label etiqueta,
        string mensaje)
    {
        etiqueta.Text = mensaje;
        etiqueta.IsVisible = true;
    }

    private void LimpiarErrores()
    {
        LimpiarEtiqueta(UsuarioErrorLabel);
        LimpiarEtiqueta(ContrasenaErrorLabel);
        LimpiarEtiqueta(NombreErrorLabel);
        LimpiarEtiqueta(DireccionErrorLabel);
        LimpiarEtiqueta(TelefonoErrorLabel);
        LimpiarEtiqueta(EmailErrorLabel);

        LimpiarMensajeGeneral();
    }

    private static void LimpiarEtiqueta(Label etiqueta)
    {
        etiqueta.Text = string.Empty;
        etiqueta.IsVisible = false;
    }

    private void LimpiarMensajeGeneral()
    {
        MensajeLabel.Text = string.Empty;
        MensajeLabel.IsVisible = false;
    }

    private void MostrarMensajeGeneral(
        string mensaje,
        bool esError)
    {
        MensajeLabel.Text = mensaje;

        MensajeLabel.TextColor = esError
            ? Color.FromArgb("#FF8A8A")
            : Color.FromArgb("#4CAF50");

        MensajeLabel.IsVisible = true;
    }
}