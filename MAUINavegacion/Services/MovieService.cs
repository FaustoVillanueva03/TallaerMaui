using System.Net.Http.Json;
using MAUINavegacion.Models;

namespace MAUINavegacion.Services;

public class MovieService
{
    private readonly HttpClient _httpClient;

    // Acá va la API Key v3, no el token largo
    private const string ApiKey = "d11a00025e66dbb1f44ffcc5aef97948";

    public MovieService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<Pelicula>> ObtenerPeliculasAsync()
    {
        string url =
            $"https://api.themoviedb.org/3/movie/popular" +
            $"?api_key={ApiKey}&language=es-ES&page=1";

        HttpResponseMessage respuestaHttp =
            await _httpClient.GetAsync(url);

        string contenido =
            await respuestaHttp.Content.ReadAsStringAsync();

        if (!respuestaHttp.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Error TMDB {(int)respuestaHttp.StatusCode}: {contenido}");
        }

        RespuestaPeliculas? respuesta =
            await respuestaHttp.Content
                .ReadFromJsonAsync<RespuestaPeliculas>();

        return respuesta?.Peliculas ?? new List<Pelicula>();
    }
}