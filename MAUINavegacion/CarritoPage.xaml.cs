using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class CarritoPage : ContentPage
{
    private readonly CarritoService _carritoService;

    public CarritoPage()
    {
        InitializeComponent();

        _carritoService = new CarritoService();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        CargarCarrito();
    }

    private void CargarCarrito()
    {
        List<CarritoItem> items =
            _carritoService.ObtenerItems();

        ListaCarrito.ItemsSource = null;
        ListaCarrito.ItemsSource = items;

        int cantidad =
            _carritoService.ObtenerCantidad();

        CantidadLabel.Text =
            cantidad == 1
                ? "1 producto"
                : $"{cantidad} productos";

        double totalUYU =
            _carritoService.ObtenerTotalUYU();

        TotalLabel.Text =
            $"$ {totalUYU:0.00} UYU";

        bool carritoVacio =
            cantidad == 0;

        ListaCarrito.IsVisible =
            !carritoVacio;

        CarritoVacioLayout.IsVisible =
            carritoVacio;

        ResumenCarrito.IsVisible =
            !carritoVacio;

        VaciarButton.IsVisible =
            !carritoVacio;
    }

    private async void OnEliminarClicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button boton ||
            boton.BindingContext is not CarritoItem item)
        {
            return;
        }

        bool confirmar =
            await DisplayAlert(
                "Eliminar producto",
                $"¿Querés eliminar \"{item.Titulo}\" del carrito?",
                "Eliminar",
                "Cancelar");

        if (!confirmar)
        {
            return;
        }

        _carritoService.Eliminar(item);

        CargarCarrito();
    }

    private async void OnVaciarClicked(
        object sender,
        EventArgs e)
    {
        bool confirmar =
            await DisplayAlert(
                "Vaciar carrito",
                "¿Querés eliminar todos los productos?",
                "Vaciar",
                "Cancelar");

        if (!confirmar)
        {
            return;
        }

        _carritoService.Vaciar();

        CargarCarrito();
    }

    private async void OnContinuarCompraClicked(
        object sender,
        EventArgs e)
    {
        int cantidad =
            _carritoService.ObtenerCantidad();

        if (cantidad == 0)
        {
            await DisplayAlert(
                "Carrito",
                "El carrito está vacío.",
                "Aceptar");

            return;
        }

        double totalUYU =
            _carritoService.ObtenerTotalUYU();

        await DisplayAlert(
            "Resumen de compra",
            $"Productos: {cantidad}\n" +
            $"Total: $ {totalUYU:0.00} UYU\n\n" +
            "La selección quedó guardada en el carrito.",
            "Aceptar");
    }

    private async void OnVerPeliculasClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PushAsync(
            new PeliculasPage());
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}