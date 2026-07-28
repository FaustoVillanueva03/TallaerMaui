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
    public async Task<Trailer?> ObtenerTrailerAsync(int peliculaId)
    {
        string urlEspanol =
            $"https://api.themoviedb.org/3/movie/{peliculaId}/videos" +
            $"?api_key={ApiKey}&language=es-ES";

        RespuestaTrailers? respuesta =
            await _httpClient.GetFromJsonAsync<RespuestaTrailers>(urlEspanol);

        Trailer? trailer = ElegirTrailer(respuesta?.Trailers);

        if (trailer != null)
            return trailer;

        // Si no encuentra uno en español, busca en inglés.
        string urlIngles =
            $"https://api.themoviedb.org/3/movie/{peliculaId}/videos" +
            $"?api_key={ApiKey}&language=en-US";

        respuesta =
            await _httpClient.GetFromJsonAsync<RespuestaTrailers>(urlIngles);

        return ElegirTrailer(respuesta?.Trailers);
    }

    private static Trailer? ElegirTrailer(List<Trailer>? videos)
    {
        if (videos == null)
            return null;

        List<Trailer> videosYoutube = videos
            .Where(video =>
                video.Sitio.Equals(
                    "YouTube",
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        return videosYoutube.FirstOrDefault(video =>
                   video.Tipo.Equals("Trailer",
                       StringComparison.OrdinalIgnoreCase)
                   && video.EsOficial)

               ?? videosYoutube.FirstOrDefault(video =>
                   video.Tipo.Equals("Trailer",
                       StringComparison.OrdinalIgnoreCase))

               ?? videosYoutube.FirstOrDefault(video =>
                   video.Tipo.Equals("Teaser",
                       StringComparison.OrdinalIgnoreCase));
    }
}