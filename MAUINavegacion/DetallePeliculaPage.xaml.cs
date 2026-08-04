using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class DetallePeliculaPage : ContentPage
{
    private Pelicula? _pelicula;

    private readonly MovieService _movieService;
    private readonly ExchangeRateService _exchangeRateService;
    private readonly CarritoService _carritoService;

    private bool _datosCargados;
    private bool _esCompra;

    private string _monedaSeleccionada = "UYU";

    private CotizacionesUYU? _cotizaciones;

    public DetallePeliculaPage()
    {
        InitializeComponent();

        _movieService = new MovieService();
        _exchangeRateService = new ExchangeRateService();
        _carritoService = new CarritoService();
    }

    public DetallePeliculaPage(
        Pelicula pelicula) : this()
    {
        _pelicula = pelicula;
        BindingContext = pelicula;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_datosCargados ||
            _pelicula == null)
        {
            return;
        }

        _datosCargados = true;
        _esCompra = false;
        _monedaSeleccionada = "UYU";

        await Task.WhenAll(
            CargarPreciosAsync(),
            CargarTrailerAsync());
    }

    private async Task CargarPreciosAsync()
    {
        try
        {
            CargandoPrecio.IsVisible = true;
            CargandoPrecio.IsRunning = true;

            _cotizaciones =
                await _exchangeRateService
                    .ObtenerValoresEnPesosAsync();

            if (_pelicula!.PrecioAlquilerUYU <= 0)
            {
                _pelicula.PrecioAlquilerUYU =
                    CalcularPrecioAlquiler(
                        _pelicula.Puntuacion);
            }

            if (_pelicula.PrecioCompraUYU <= 0)
            {
                _pelicula.PrecioCompraUYU =
                    CalcularPrecioCompra(
                        _pelicula.Puntuacion);
            }

            CalcularConversiones();
            MostrarPrecioSeleccionado();

            MensajeErrorPrecio.IsVisible = false;
            SeccionPrecio.IsVisible = true;
        }
        catch (Exception error)
        {
            SeccionPrecio.IsVisible = false;

            MensajeErrorPrecio.Text =
                $"No se pudieron cargar los precios: " +
                $"{error.Message}";

            MensajeErrorPrecio.IsVisible = true;
        }
        finally
        {
            CargandoPrecio.IsRunning = false;
            CargandoPrecio.IsVisible = false;
        }
    }

    private void CalcularConversiones()
    {
        if (_pelicula == null ||
            _cotizaciones == null)
        {
            return;
        }

        _pelicula.PrecioAlquilerUSD =
            _pelicula.PrecioAlquilerUYU /
            _cotizaciones.Dolar;

        _pelicula.PrecioAlquilerEUR =
            _pelicula.PrecioAlquilerUYU /
            _cotizaciones.Euro;

        _pelicula.PrecioAlquilerBRL =
            _pelicula.PrecioAlquilerUYU /
            _cotizaciones.Real;

        _pelicula.PrecioCompraUSD =
            _pelicula.PrecioCompraUYU /
            _cotizaciones.Dolar;

        _pelicula.PrecioCompraEUR =
            _pelicula.PrecioCompraUYU /
            _cotizaciones.Euro;

        _pelicula.PrecioCompraBRL =
            _pelicula.PrecioCompraUYU /
            _cotizaciones.Real;
    }

    private void MostrarPrecioSeleccionado()
    {
        if (_pelicula == null)
        {
            return;
        }

        ModalidadSeleccionadaLabel.Text =
            _esCompra
                ? "Precio de compra"
                : "Precio de alquiler";

        PrecioPrincipalLabel.Text =
            ObtenerPrecioTextoSeleccionado();

        ActualizarBotonesModalidad();
        ActualizarBotonesMoneda();
    }

    private string ObtenerPrecioTextoSeleccionado()
    {
        if (_pelicula == null)
        {
            return string.Empty;
        }

        if (_esCompra)
        {
            return _monedaSeleccionada switch
            {
                "USD" => _pelicula.PrecioCompraUSDTexto,
                "EUR" => _pelicula.PrecioCompraEURTexto,
                "BRL" => _pelicula.PrecioCompraBRLTexto,
                _ => _pelicula.PrecioCompraUYUTexto
            };
        }

        return _monedaSeleccionada switch
        {
            "USD" => _pelicula.PrecioAlquilerUSDTexto,
            "EUR" => _pelicula.PrecioAlquilerEURTexto,
            "BRL" => _pelicula.PrecioAlquilerBRLTexto,
            _ => _pelicula.PrecioAlquilerUYUTexto
        };
    }

    private double ObtenerPrecioNumericoSeleccionado()
    {
        if (_pelicula == null)
        {
            return 0;
        }

        if (_esCompra)
        {
            return _monedaSeleccionada switch
            {
                "USD" => _pelicula.PrecioCompraUSD,
                "EUR" => _pelicula.PrecioCompraEUR,
                "BRL" => _pelicula.PrecioCompraBRL,
                _ => _pelicula.PrecioCompraUYU
            };
        }

        return _monedaSeleccionada switch
        {
            "USD" => _pelicula.PrecioAlquilerUSD,
            "EUR" => _pelicula.PrecioAlquilerEUR,
            "BRL" => _pelicula.PrecioAlquilerBRL,
            _ => _pelicula.PrecioAlquilerUYU
        };
    }

    private double ObtenerPrecioUYUSeleccionado()
    {
        if (_pelicula == null)
        {
            return 0;
        }

        return _esCompra
            ? _pelicula.PrecioCompraUYU
            : _pelicula.PrecioAlquilerUYU;
    }

    private void ActualizarBotonesModalidad()
    {
        AplicarEstadoBoton(
            AlquilarButton,
            !_esCompra);

        AplicarEstadoBoton(
            ComprarButton,
            _esCompra);
    }

    private void ActualizarBotonesMoneda()
    {
        AplicarEstadoBoton(
            UYUButton,
            _monedaSeleccionada == "UYU");

        AplicarEstadoBoton(
            USDButton,
            _monedaSeleccionada == "USD");

        AplicarEstadoBoton(
            EURButton,
            _monedaSeleccionada == "EUR");

        AplicarEstadoBoton(
            BRLButton,
            _monedaSeleccionada == "BRL");
    }

    private static void AplicarEstadoBoton(
        Button boton,
        bool seleccionado)
    {
        Color colorPrimario =
            (Color)Application.Current!
                .Resources["ColorPrimario"];

        Color colorSuperficie =
            (Color)Application.Current!
                .Resources["ColorSuperficie"];

        Color colorTextoPrincipal =
            (Color)Application.Current!
                .Resources["ColorTextoPrincipal"];

        boton.BackgroundColor =
            seleccionado
                ? colorPrimario
                : colorSuperficie;

        boton.TextColor =
            seleccionado
                ? colorTextoPrincipal
                : colorPrimario;
    }

    private void OnAlquilarClicked(
        object sender,
        EventArgs e)
    {
        _esCompra = false;

        MostrarPrecioSeleccionado();
    }

    private void OnComprarClicked(
        object sender,
        EventArgs e)
    {
        _esCompra = true;

        MostrarPrecioSeleccionado();
    }

    private void OnUYUClicked(
        object sender,
        EventArgs e)
    {
        SeleccionarMoneda("UYU");
    }

    private void OnUSDClicked(
        object sender,
        EventArgs e)
    {
        SeleccionarMoneda("USD");
    }

    private void OnEURClicked(
        object sender,
        EventArgs e)
    {
        SeleccionarMoneda("EUR");
    }

    private void OnBRLClicked(
        object sender,
        EventArgs e)
    {
        SeleccionarMoneda("BRL");
    }

    private void SeleccionarMoneda(
        string moneda)
    {
        _monedaSeleccionada = moneda;

        MostrarPrecioSeleccionado();
    }

    private async void OnAgregarCarritoClicked(
        object sender,
        EventArgs e)
    {
        if (_pelicula == null)
        {
            return;
        }

        if (_cotizaciones == null)
        {
            await DisplayAlert(
                "Carrito",
                "Esperá a que se carguen los precios.",
                "Aceptar");

            return;
        }

        double precioSeleccionado =
            ObtenerPrecioNumericoSeleccionado();

        double precioUYU =
            ObtenerPrecioUYUSeleccionado();

        CarritoItem item = new()
        {
            IdContenido = _pelicula.Id,
            Titulo = _pelicula.Titulo,
            Imagen = _pelicula.ImagenCompleta,
            TipoContenido = "Película",
            Modalidad = _esCompra
                ? "Compra"
                : "Alquiler",
            Moneda = _monedaSeleccionada,
            Precio = precioSeleccionado,
            PrecioUYU = precioUYU
        };

        bool agregado =
            _carritoService.Agregar(item);

        MensajeCarritoLabel.Text =
            agregado
                ? "Película agregada al carrito."
                : "La selección del carrito fue actualizada.";

        MensajeCarritoLabel.IsVisible = true;

        AgregarCarritoButton.Text =
            "✓ Agregado al carrito";

        AgregarCarritoButton.IsEnabled = false;

        await Task.Delay(1600);

        MensajeCarritoLabel.IsVisible = false;

        AgregarCarritoButton.Text =
            "🛒 Agregar al carrito";

        AgregarCarritoButton.IsEnabled = true;
    }

    private static double CalcularPrecioAlquiler(
        double puntuacion)
    {
        if (puntuacion >= 8)
        {
            return 450;
        }

        if (puntuacion >= 7)
        {
            return 390;
        }

        return 320;
    }

    private static double CalcularPrecioCompra(
        double puntuacion)
    {
        if (puntuacion >= 8)
        {
            return 1200;
        }

        if (puntuacion >= 7)
        {
            return 990;
        }

        return 790;
    }

    private async Task CargarTrailerAsync()
    {
        try
        {
            Trailer? trailer =
                await _movieService
                    .ObtenerTrailerAsync(
                        _pelicula!.Id);

            if (trailer == null ||
                string.IsNullOrWhiteSpace(
                    trailer.Key))
            {
                MensajeSinTrailer.Text =
                    "No hay un tráiler disponible " +
                    "para esta película.";

                MensajeSinTrailer.IsVisible = true;
                SeccionTrailer.IsVisible = false;

                return;
            }

            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta name=""viewport""
          content=""width=device-width, initial-scale=1.0"">

    <style>
        html, body {{
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
            background-color: black;
            overflow: hidden;
        }}

        iframe {{
            width: 100%;
            height: 100%;
            border: 0;
        }}
    </style>
</head>

<body>
    <iframe
        src=""https://www.youtube-nocookie.com/embed/{trailer.Key}?playsinline=1&rel=0""
        title=""YouTube video player""
        allow=""accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share""
        allowfullscreen>
    </iframe>
</body>
</html>";

            TrailerWebView.Source =
                new HtmlWebViewSource
                {
                    Html = html
                };

            MensajeSinTrailer.IsVisible = false;
            SeccionTrailer.IsVisible = true;
        }
        catch (Exception error)
        {
            MensajeSinTrailer.Text =
                $"No se pudo cargar el tráiler: " +
                $"{error.Message}";

            MensajeSinTrailer.IsVisible = true;
            SeccionTrailer.IsVisible = false;
        }
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}