using MAUINavegacion.Models;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;

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

    public async Task<RespuestaPronostico?> ObtenerPronosticoAsync()
    {
        string url =
            $"https://api.openweathermap.org/data/2.5/forecast" +
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
            .ReadFromJsonAsync<RespuestaPronostico>();
    }


    public async Task<List<PronosticoDia>> ObtenerPronostico5DiasAsync()
    {
        RespuestaPronostico? respuesta =
            await ObtenerPronosticoAsync();

        List<PronosticoDia> resultado = new();

        if (respuesta == null)
            return resultado;

        var dias = respuesta.Lista
            .Where(item => item.Fecha.Contains("12:00:00"))
            .Take(5);

        foreach (var item in dias)
        {
            DateTime fecha = DateTime.Parse(item.Fecha);

            string dia =
                fecha.ToString(
                    "dddd",
                    new CultureInfo("es-ES"));

            dia =
                char.ToUpper(dia[0]) +
                dia.Substring(1);

            resultado.Add(new PronosticoDia
            {
                Dia = dia,
                Temperatura = item.Main.Temperatura,
                Descripcion = item.Weather[0].Descripcion,
                Icono = item.Weather[0].Icono
            });
        }

        return resultado;
    }
}