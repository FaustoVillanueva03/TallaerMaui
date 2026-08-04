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
            _carritoService
                .ObtenerTotalUYUSeleccionado();

        double totalUSD =
            _carritoService
                .ObtenerTotalUSD();

        double totalEUR =
            _carritoService
                .ObtenerTotalEUR();

        double totalBRL =
            _carritoService
                .ObtenerTotalBRL();

        TotalUYULabel.Text =
            $"$ {totalUYU:0.00} UYU";

        TotalUSDLabel.Text =
            $"USD {totalUSD:0.00}";

        TotalEURLabel.Text =
            $"EUR {totalEUR:0.00}";

        TotalBRLLabel.Text =
            $"BRL {totalBRL:0.00}";

        TotalUYUBorder.IsVisible =
            totalUYU > 0;

        TotalUSDBorder.IsVisible =
            totalUSD > 0;

        TotalEURBorder.IsVisible =
            totalEUR > 0;

        TotalBRLBorder.IsVisible =
            totalBRL > 0;

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
            _carritoService
                .ObtenerTotalUYUSeleccionado();

        double totalUSD =
            _carritoService
                .ObtenerTotalUSD();

        double totalEUR =
            _carritoService
                .ObtenerTotalEUR();

        double totalBRL =
            _carritoService
                .ObtenerTotalBRL();

        string resumen =
            CrearResumenMonedas(
                totalUYU,
                totalUSD,
                totalEUR,
                totalBRL);

        await DisplayAlert(
            "Resumen de compra",
            $"Productos: {cantidad}\n\n" +
            $"{resumen}",
            "Aceptar");
    }

    private static string CrearResumenMonedas(
        double totalUYU,
        double totalUSD,
        double totalEUR,
        double totalBRL)
    {
        List<string> lineas = new();

        if (totalUYU > 0)
        {
            lineas.Add(
                $"Pesos uruguayos: $ {totalUYU:0.00} UYU");
        }

        if (totalUSD > 0)
        {
            lineas.Add(
                $"Dólares: USD {totalUSD:0.00}");
        }

        if (totalEUR > 0)
        {
            lineas.Add(
                $"Euros: EUR {totalEUR:0.00}");
        }

        if (totalBRL > 0)
        {
            lineas.Add(
                $"Reales: BRL {totalBRL:0.00}");
        }

        return string.Join(
            "\n",
            lineas);
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