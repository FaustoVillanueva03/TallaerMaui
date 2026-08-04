using System.Net.Http.Json;
using MAUINavegacion.Models;

namespace MAUINavegacion.Services;

public class ExchangeRateService
{
    private readonly HttpClient _httpClient;

    private const string ApiKey =
        "01cc1cd00a570e82de6c1946";

    private const string ClaveFecha =
        "FechaCotizaciones";

    private const string ClaveDolar =
        "CotizacionDolar";

    private const string ClaveEuro =
        "CotizacionEuro";

    private const string ClaveReal =
        "CotizacionReal";

    public ExchangeRateService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<RespuestaCotizacion?>
        ObtenerCotizacionesAsync()
    {
        string fechaGuardada =
            Preferences.Default.Get(
                ClaveFecha,
                string.Empty);

        string fechaActual =
            DateTime.Today.ToString("yyyy-MM-dd");

        bool existenCotizaciones =
            Preferences.Default.ContainsKey(
                ClaveDolar) &&
            Preferences.Default.ContainsKey(
                ClaveEuro) &&
            Preferences.Default.ContainsKey(
                ClaveReal);

        if (fechaGuardada == fechaActual &&
            existenCotizaciones)
        {
            double dolarUYU =
                Preferences.Default.Get(
                    ClaveDolar,
                    0.0);

            double euroUYU =
                Preferences.Default.Get(
                    ClaveEuro,
                    0.0);

            double realUYU =
                Preferences.Default.Get(
                    ClaveReal,
                    0.0);

            if (dolarUYU > 0 &&
                euroUYU > 0 &&
                realUYU > 0)
            {
                return CrearRespuestaDesdeUYU(
                    dolarUYU,
                    euroUYU,
                    realUYU);
            }
        }

        string url =
            $"https://v6.exchangerate-api.com/v6/" +
            $"{ApiKey}/latest/UYU";

        HttpResponseMessage respuestaHttp =
            await _httpClient.GetAsync(url);

        string contenido =
            await respuestaHttp.Content
                .ReadAsStringAsync();

        if (!respuestaHttp.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Error ExchangeRate " +
                $"{(int)respuestaHttp.StatusCode}: " +
                contenido);
        }

        RespuestaCotizacion? respuesta =
            await respuestaHttp.Content
                .ReadFromJsonAsync<RespuestaCotizacion>();

        if (respuesta == null ||
            respuesta.ConversionRates.Dolar <= 0 ||
            respuesta.ConversionRates.Euro <= 0 ||
            respuesta.ConversionRates.Real <= 0)
        {
            throw new Exception(
                "La API devolvió cotizaciones inválidas.");
        }

        double dolar =
            1 / respuesta.ConversionRates.Dolar;

        double euro =
            1 / respuesta.ConversionRates.Euro;

        double real =
            1 / respuesta.ConversionRates.Real;

        Preferences.Default.Set(
            ClaveFecha,
            fechaActual);

        Preferences.Default.Set(
            ClaveDolar,
            dolar);

        Preferences.Default.Set(
            ClaveEuro,
            euro);

        Preferences.Default.Set(
            ClaveReal,
            real);

        return respuesta;
    }

    public async Task<CotizacionesUYU>
        ObtenerValoresEnPesosAsync()
    {
        RespuestaCotizacion? respuesta =
            await ObtenerCotizacionesAsync();

        if (respuesta == null)
        {
            throw new Exception(
                "No se pudieron obtener las cotizaciones.");
        }

        if (respuesta.ConversionRates.Dolar <= 0 ||
            respuesta.ConversionRates.Euro <= 0 ||
            respuesta.ConversionRates.Real <= 0)
        {
            throw new Exception(
                "Las cotizaciones recibidas no son válidas.");
        }

        return new CotizacionesUYU
        {
            Dolar =
                1 / respuesta.ConversionRates.Dolar,

            Euro =
                1 / respuesta.ConversionRates.Euro,

            Real =
                1 / respuesta.ConversionRates.Real
        };
    }

    private static RespuestaCotizacion
        CrearRespuestaDesdeUYU(
            double dolarUYU,
            double euroUYU,
            double realUYU)
    {
        return new RespuestaCotizacion
        {
            ConversionRates =
                new ConversionRates
                {
                    Dolar = 1 / dolarUYU,
                    Euro = 1 / euroUYU,
                    Real = 1 / realUYU
                }
        };
    }
}

public class CotizacionesUYU
{
    public double Dolar { get; set; }

    public double Euro { get; set; }

    public double Real { get; set; }
}