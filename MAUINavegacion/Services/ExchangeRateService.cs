using System.Net.Http;
using System.Net.Http.Json;
using MAUINavegacion.Models;

namespace MAUINavegacion.Services;

public class ExchangeRateService
{
    private readonly HttpClient _httpClient;

    
    private const string ApiKey =
        "01cc1cd00a570e82de6c1946";

    public ExchangeRateService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<RespuestaCotizacion?> ObtenerCotizacionesAsync()
    {
        string url =
            $"https://v6.exchangerate-api.com/v6/{ApiKey}/latest/UYU";

        return await _httpClient
            .GetFromJsonAsync<RespuestaCotizacion>(url);
    }
}
