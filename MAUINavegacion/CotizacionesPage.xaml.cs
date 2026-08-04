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

        try
        {
            Cargando.IsVisible = true;
            Cargando.IsRunning = true;

            RespuestaCotizacion? cotizacion =
                await _exchangeRateService
                    .ObtenerCotizacionesAsync();

            if (cotizacion == null)
            {
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

            _cotizacionesCargadas = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                ex.Message,
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

        if (!double.TryParse(
                MontoEntry.Text,
                out double monto) ||
            monto < 0)
        {
            await DisplayAlert(
                "Error",
                "Ingresá un monto válido.",
                "Aceptar");

            return;
        }

        string moneda =
            MonedaPicker.SelectedItem?
                .ToString() ?? string.Empty;

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

        ResultadoUYU.IsVisible = true;
        ResultadoUSD.IsVisible = true;
        ResultadoEUR.IsVisible = true;
        ResultadoBRL.IsVisible = true;

        switch (moneda)
        {
            case "Peso Uruguayo":
                ResultadoUYU.IsVisible = false;
                break;

            case "Dólar":
                ResultadoUSD.IsVisible = false;
                break;

            case "Euro":
                ResultadoEUR.IsVisible = false;
                break;

            case "Real":
                ResultadoBRL.IsVisible = false;
                break;
        }

        ResultadoUYU.Text =
            $"UYU: {uyu:0.00}";

        ResultadoUSD.Text =
            $"USD: {usd:0.00}";

        ResultadoEUR.Text =
            $"EUR: {eur:0.00}";

        ResultadoBRL.Text =
            $"BRL: {brl:0.00}";
    }

    private async void OnVolverClicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopAsync();
    }
}