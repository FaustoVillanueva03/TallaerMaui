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
            MostrarMensaje(
                "No se pudo seleccionar la foto.");
        }
    }

    private async void OnRegistrarClicked(
        object sender,
        EventArgs e)
    {
        MensajeLabel.IsVisible = false;

        await App.Database.InicializarAsync();

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

        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrWhiteSpace(contrasena) ||
            string.IsNullOrWhiteSpace(nombreCompleto) ||
            string.IsNullOrWhiteSpace(direccion) ||
            string.IsNullOrWhiteSpace(telefono) ||
            string.IsNullOrWhiteSpace(email))
        {
            MostrarMensaje(
                "Completá todos los datos obligatorios.");

            return;
        }

        if (nombreUsuario.Length < 3)
        {
            MostrarMensaje(
                "El usuario debe tener al menos 3 caracteres.");

            return;
        }

        if (contrasena.Length < 4)
        {
            MostrarMensaje(
                "La contraseña debe tener al menos 4 caracteres.");

            return;
        }

        if (!Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            MostrarMensaje(
                "Ingresá un email válido.");

            return;
        }

        if (!Regex.IsMatch(
                telefono,
                @"^[0-9+\-\s]{6,20}$"))
        {
            MostrarMensaje(
                "Ingresá un teléfono válido.");

            return;
        }

        try
        {
            Usuario? usuarioExistente =
                await App.Database
                    .ObtenerUsuarioPorNombreAsync(
                        nombreUsuario);

            if (usuarioExistente != null)
            {
                MostrarMensaje(
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

            await DisplayAlert(
                "Registro correcto",
                "El usuario se registró correctamente.",
                "Aceptar");

            await Navigation.PopAsync();
        }
        catch (SQLiteException error)
        {
            if (error.Message.Contains(
                    "UNIQUE",
                    StringComparison.OrdinalIgnoreCase))
            {
                MostrarMensaje(
                    "El usuario o el email ya están registrados.");
            }
            else
            {
                MostrarMensaje(
                    "No se pudo guardar el usuario.");
            }
        }
        catch (Exception)
        {
            MostrarMensaje(
                "Ocurrió un error al registrar el usuario.");
        }
    }

    private void MostrarMensaje(string mensaje)
    {
        MensajeLabel.Text = mensaje;
        MensajeLabel.IsVisible = true;
    }
}