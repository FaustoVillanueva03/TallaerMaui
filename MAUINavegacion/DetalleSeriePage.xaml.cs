using MAUINavegacion.Models;
using MAUINavegacion.Services;


namespace MAUINavegacion;

public partial class DetalleSeriePage : BasePage
{
    private readonly SerieService _serieService;
    private readonly ExchangeRateService _exchangeRateService;
    private readonly CarritoService _carritoService;

    private Serie? _serie;
    private bool _datosCargados;
    private bool _esCompra;

    private string _monedaSeleccionada = "UYU";

    private CotizacionesUYU? _cotizaciones;

    public DetalleSeriePage()
    {
        InitializeComponent();

        _serieService = new SerieService();
        _exchangeRateService = new ExchangeRateService();
        _carritoService = new CarritoService();
    }

    public DetalleSeriePage(
        Serie serie) : this()
    {
        _serie = serie;
        BindingContext = serie;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_datosCargados ||
            _serie == null)
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

            if (_serie!.PrecioAlquilerUYU <= 0)
            {
                _serie.PrecioAlquilerUYU =
                    CalcularPrecioAlquiler(
                        _serie.Puntuacion);
            }

            if (_serie.PrecioCompraUYU <= 0)
            {
                _serie.PrecioCompraUYU =
                    CalcularPrecioCompra(
                        _serie.Puntuacion);
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
        if (_serie == null ||
            _cotizaciones == null)
        {
            return;
        }

        _serie.PrecioAlquilerUSD =
            _serie.PrecioAlquilerUYU /
            _cotizaciones.Dolar;

        _serie.PrecioAlquilerEUR =
            _serie.PrecioAlquilerUYU /
            _cotizaciones.Euro;

        _serie.PrecioAlquilerBRL =
            _serie.PrecioAlquilerUYU /
            _cotizaciones.Real;

        _serie.PrecioCompraUSD =
            _serie.PrecioCompraUYU /
            _cotizaciones.Dolar;

        _serie.PrecioCompraEUR =
            _serie.PrecioCompraUYU /
            _cotizaciones.Euro;

        _serie.PrecioCompraBRL =
            _serie.PrecioCompraUYU /
            _cotizaciones.Real;
    }

    private void MostrarPrecioSeleccionado()
    {
        if (_serie == null)
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
        if (_serie == null)
        {
            return string.Empty;
        }

        if (_esCompra)
        {
            return _monedaSeleccionada switch
            {
                "USD" => _serie.PrecioCompraUSDTexto,
                "EUR" => _serie.PrecioCompraEURTexto,
                "BRL" => _serie.PrecioCompraBRLTexto,
                _ => _serie.PrecioCompraUYUTexto
            };
        }

        return _monedaSeleccionada switch
        {
            "USD" => _serie.PrecioAlquilerUSDTexto,
            "EUR" => _serie.PrecioAlquilerEURTexto,
            "BRL" => _serie.PrecioAlquilerBRLTexto,
            _ => _serie.PrecioAlquilerUYUTexto
        };
    }

    private double ObtenerPrecioNumericoSeleccionado()
    {
        if (_serie == null)
        {
            return 0;
        }

        if (_esCompra)
        {
            return _monedaSeleccionada switch
            {
                "USD" => _serie.PrecioCompraUSD,
                "EUR" => _serie.PrecioCompraEUR,
                "BRL" => _serie.PrecioCompraBRL,
                _ => _serie.PrecioCompraUYU
            };
        }

        return _monedaSeleccionada switch
        {
            "USD" => _serie.PrecioAlquilerUSD,
            "EUR" => _serie.PrecioAlquilerEUR,
            "BRL" => _serie.PrecioAlquilerBRL,
            _ => _serie.PrecioAlquilerUYU
        };
    }

    private double ObtenerPrecioUYUSeleccionado()
    {
        if (_serie == null)
        {
            return 0;
        }

        return _esCompra
            ? _serie.PrecioCompraUYU
            : _serie.PrecioAlquilerUYU;
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
        if (_serie == null)
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
            IdContenido = _serie.Id,
            Titulo = _serie.Nombre,
            Imagen = _serie.ImagenCompleta,
            TipoContenido = "Serie",
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
                ? "Serie agregada al carrito."
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
            return 420;
        }

        if (puntuacion >= 7)
        {
            return 350;
        }

        return 280;
    }

    private static double CalcularPrecioCompra(
        double puntuacion)
    {
        if (puntuacion >= 8)
        {
            return 1100;
        }

        if (puntuacion >= 7)
        {
            return 900;
        }

        return 700;
    }

    private async Task CargarTrailerAsync()
    {
        try
        {
            Trailer? trailer =
                await _serieService
                    .ObtenerTrailerAsync(
                        _serie!.Id);

            if (trailer == null ||
                string.IsNullOrWhiteSpace(
                    trailer.Key))
            {
                MensajeSinTrailer.Text =
                    "No hay un tráiler disponible " +
                    "para esta serie.";

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