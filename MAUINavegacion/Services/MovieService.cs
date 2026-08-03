using System.Net.Http.Json;
using MAUINavegacion.Models;

namespace MAUINavegacion.Services;

public class MovieService
{
    private readonly HttpClient _httpClient;

    private const string ApiKey =
        "d11a00025e66dbb1f44ffcc5aef97948";

    public MovieService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<Pelicula>> ObtenerPeliculasAsync()
    {
        List<Pelicula> todasLasPeliculas = new();

        const int cantidadPaginas = 5;

        for (int pagina = 1;
             pagina <= cantidadPaginas;
             pagina++)
        {
            string url =
                $"https://api.themoviedb.org/3/discover/movie" +
                $"?api_key={ApiKey}" +
                $"&language=es-ES" +
                $"&region=UY" +
                $"&include_adult=false" +
                $"&include_video=false" +
                $"&sort_by=primary_release_date.desc" +
                $"&vote_count.gte=10" +
                $"&page={pagina}";

            HttpResponseMessage respuestaHttp =
                await _httpClient.GetAsync(url);

            string contenido =
                await respuestaHttp.Content
                    .ReadAsStringAsync();

            if (!respuestaHttp.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Error TMDB {(int)respuestaHttp.StatusCode}: {contenido}");
            }

            RespuestaPeliculas? respuesta =
                await respuestaHttp.Content
                    .ReadFromJsonAsync<RespuestaPeliculas>();

            if (respuesta?.Peliculas != null)
            {
                todasLasPeliculas.AddRange(
                    respuesta.Peliculas);
            }
        }

        return todasLasPeliculas
            .GroupBy(pelicula => pelicula.Id)
            .Select(grupo => grupo.First())
            .ToList();
    }

    public async Task<List<Pelicula>>
        ObtenerPeliculasPorGeneroAsync(
            int generoId,
            int cantidad = 10)
    {
        string url =
            $"https://api.themoviedb.org/3/discover/movie" +
            $"?api_key={ApiKey}" +
            $"&language=es-ES" +
            $"&region=UY" +
            $"&include_adult=false" +
            $"&include_video=false" +
            $"&sort_by=popularity.desc" +
            $"&vote_count.gte=50" +
            $"&with_genres={generoId}" +
            $"&page=1";

        HttpResponseMessage respuestaHttp =
            await _httpClient.GetAsync(url);

        string contenido =
            await respuestaHttp.Content
                .ReadAsStringAsync();

        if (!respuestaHttp.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Error TMDB {(int)respuestaHttp.StatusCode}: {contenido}");
        }

        RespuestaPeliculas? respuesta =
            await respuestaHttp.Content
                .ReadFromJsonAsync<RespuestaPeliculas>();

        if (respuesta?.Peliculas == null)
        {
            return new List<Pelicula>();
        }

        return respuesta.Peliculas
            .GroupBy(pelicula => pelicula.Id)
            .Select(grupo => grupo.First())
            .Take(cantidad)
            .ToList();
    }

    public async Task<Trailer?> ObtenerTrailerAsync(
        int peliculaId)
    {
        string urlEspanol =
            $"https://api.themoviedb.org/3/movie/{peliculaId}/videos" +
            $"?api_key={ApiKey}" +
            $"&language=es-ES";

        RespuestaTrailers? respuesta =
            await _httpClient
                .GetFromJsonAsync<RespuestaTrailers>(
                    urlEspanol);

        Trailer? trailer =
            ElegirTrailer(respuesta?.Trailers);

        if (trailer != null)
        {
            return trailer;
        }

        string urlIngles =
            $"https://api.themoviedb.org/3/movie/{peliculaId}/videos" +
            $"?api_key={ApiKey}" +
            $"&language=en-US";

        respuesta =
            await _httpClient
                .GetFromJsonAsync<RespuestaTrailers>(
                    urlIngles);

        return ElegirTrailer(respuesta?.Trailers);
    }

    private static Trailer? ElegirTrailer(
        List<Trailer>? videos)
    {
        if (videos == null)
        {
            return null;
        }

        List<Trailer> videosYoutube = videos
            .Where(video =>
                !string.IsNullOrWhiteSpace(video.Sitio) &&
                video.Sitio.Equals(
                    "YouTube",
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        return videosYoutube.FirstOrDefault(video =>
                   !string.IsNullOrWhiteSpace(video.Tipo) &&
                   video.Tipo.Equals(
                       "Trailer",
                       StringComparison.OrdinalIgnoreCase) &&
                   video.EsOficial)

               ?? videosYoutube.FirstOrDefault(video =>
                   !string.IsNullOrWhiteSpace(video.Tipo) &&
                   video.Tipo.Equals(
                       "Trailer",
                       StringComparison.OrdinalIgnoreCase))

               ?? videosYoutube.FirstOrDefault(video =>
                   !string.IsNullOrWhiteSpace(video.Tipo) &&
                   video.Tipo.Equals(
                       "Teaser",
                       StringComparison.OrdinalIgnoreCase));
    }
}