using System.Globalization;
using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion;

public partial class CotizacionesPage : BasePage
{
    private readonly ExchangeRateService _exchangeRateService;

    private double _dolar;
    private double _euro;
    private double _real;

    private bool _cotizacionesCargadas;

    public CotizacionesPage()
    {
        InitializeComponent();

        _exchangeRateService =
            new ExchangeRateService();

        MonedaPicker.Items.Add(
            "Peso Uruguayo");

        MonedaPicker.Items.Add(
            "Dólar");

        MonedaPicker.Items.Add(
            "Euro");

        MonedaPicker.Items.Add(
            "Real");

        MonedaPicker.SelectedIndex = 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_cotizacionesCargadas)
        {
            return;
        }

        await CargarCotizacionesAsync();
    }

    private async Task CargarCotizacionesAsync()
    {
        try
        {
            Cargando.IsVisible = true;
            Cargando.IsRunning = true;

            FechaActualizacionLabel.Text =
                "Actualizando cotizaciones...";

            RespuestaCotizacion? cotizacion =
                await _exchangeRateService
                    .ObtenerCotizacionesAsync();

            if (cotizacion == null)
            {
                FechaActualizacionLabel.Text =
                    "No se pudieron actualizar las cotizaciones.";

                await DisplayAlert(
                    "Cotizaciones",
                    "No se pudieron obtener las cotizaciones.",
                    "Aceptar");

                return;
            }

            _dolar =
                1 /
                cotizacion.ConversionRates.Dolar;

            _euro =
                1 /
                cotizacion.ConversionRates.Euro;

            _real =
                1 /
                cotizacion.ConversionRates.Real;

            DolarLabel.Text =
                $"1 USD = {_dolar:0.00} UYU";

            EuroLabel.Text =
                $"1 EUR = {_euro:0.00} UYU";

            RealLabel.Text =
                $"1 BRL = {_real:0.00} UYU";

            FechaActualizacionLabel.Text =
                $"Última actualización: {DateTime.Now:dd/MM/yyyy HH:mm}";

            _cotizacionesCargadas = true;
        }
        catch (Exception error)
        {
            FechaActualizacionLabel.Text =
                "Ocurrió un error al actualizar.";

            await DisplayAlert(
                "Error",
                error.Message,
                "Aceptar");
        }
        finally
        {
            Cargando.IsRunning = false;
            Cargando.IsVisible = false;
        }
    }

    private async void OnConvertirClicked(
        object sender,
        EventArgs e)
    {
        if (_dolar <= 0 ||
            _euro <= 0 ||
            _real <= 0)
        {
            await DisplayAlert(
                "Conversor",
                "Esperá a que se carguen las cotizaciones.",
                "Aceptar");

            return;
        }

        if (!IntentarObtenerMonto(
                MontoEntry.Text,
                out double monto) ||
            monto < 0)
        {
            await DisplayAlert(
                "Monto incorrecto",
                "Ingresá un número válido mayor o igual a cero.",
                "Aceptar");

            return;
        }

        string moneda =
            MonedaPicker.SelectedItem?
                .ToString() ??
            string.Empty;

        if (string.IsNullOrWhiteSpace(moneda))
        {
            await DisplayAlert(
                "Moneda",
                "Seleccioná una moneda.",
                "Aceptar");

            return;
        }

        double uyu;

        switch (moneda)
        {
            case "Dólar":
                uyu = monto * _dolar;
                break;

            case "Euro":
                uyu = monto * _euro;
                break;

            case "Real":
                uyu = monto * _real;
                break;

            default:
                uyu = monto;
                break;
        }

        double usd =
            uyu / _dolar;

        double eur =
            uyu / _euro;

        double brl =
            uyu / _real;

        ResultadoUYU.Text =
            $"$ {uyu:0.00} UYU";

        ResultadoUSD.Text =
            $"USD {usd:0.00}";

        ResultadoEUR.Text =
            $"EUR {eur:0.00}";

        ResultadoBRL.Text =
            $"BRL {brl:0.00}";

        MontoOriginalLabel.Text =
            $"Conversión de {monto:0.00} {ObtenerCodigoMoneda(moneda)}";

        ResultadoUYUBorder.IsVisible =
            moneda != "Peso Uruguayo";

        ResultadoUSDBorder.IsVisible =
            moneda != "Dólar";

        ResultadoEURBorder.IsVisible =
            moneda != "Euro";

        ResultadoBRLBorder.IsVisible =
            moneda != "Real";

        ResultadoBorder.IsVisible = true;
    }

    private static bool IntentarObtenerMonto(
        string? texto,
        out double monto)
    {
        monto = 0;

        if (string.IsNullOrWhiteSpace(texto))
        {
            return false;
        }

        if (double.TryParse(
                texto,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out monto))
        {
            return true;
        }

        string textoNormalizado =
            texto.Replace(
                ',',
                '.');

        return double.TryParse(
            textoNormalizado,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out monto);
    }

    private static string ObtenerCodigoMoneda(
        string moneda)
    {
        return moneda switch
        {
            "Dólar" => "USD",
            "Euro" => "EUR",
            "Real" => "BRL",
            _ => "UYU"
        };
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}