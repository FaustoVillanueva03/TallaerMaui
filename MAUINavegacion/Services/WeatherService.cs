using System.Net.Http;
using System.Net.Http.Json;
using MAUINavegacion.Models;

namespace MAUINavegacion.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;

    private const string ApiKey =
        "0455daa1e9a4e70e228e66771d5c98f3";

    public WeatherService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<RespuestaClima?> ObtenerClimaActualAsync()
    {
        string url =
            $"https://api.openweathermap.org/data/2.5/weather" +
            $"?q=Punta del Este,UY" +
            $"&appid={ApiKey}" +
            $"&units=metric" +
            $"&lang=es";

        HttpResponseMessage respuestaHttp =
            await _httpClient.GetAsync(url);

        string contenido =
            await respuestaHttp.Content.ReadAsStringAsync();

        if (!respuestaHttp.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Error OpenWeather {(int)respuestaHttp.StatusCode}: {contenido}");
        }

        return await respuestaHttp.Content
            .ReadFromJsonAsync<RespuestaClima>();
    }
}