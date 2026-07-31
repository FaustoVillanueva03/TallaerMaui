using System.Net.Http.Json;
using MAUINavegacion.Models;

namespace MAUINavegacion.Services;

public class SerieService
{
    private readonly HttpClient _httpClient;

    private const string ApiKey =
        "d11a00025e66dbb1f44ffcc5aef97948";

    public SerieService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<Serie>> ObtenerSeriesAsync()
    {
        List<Serie> todasLasSeries = new();

        const int cantidadPaginas = 5;

        for (int pagina = 1;
             pagina <= cantidadPaginas;
             pagina++)
        {
            string url =
                $"https://api.themoviedb.org/3/discover/tv" +
                $"?api_key={ApiKey}" +
                $"&language=es-ES" +
                $"&include_adult=false" +
                $"&sort_by=first_air_date.desc" +
                $"&vote_count.gte=10" +
                $"&page={pagina}";

            HttpResponseMessage respuestaHttp =
                await _httpClient.GetAsync(url);

            string contenido =
                await respuestaHttp.Content.ReadAsStringAsync();

            if (!respuestaHttp.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Error TMDB {(int)respuestaHttp.StatusCode}: " +
                    contenido);
            }

            RespuestaSeries? respuesta =
                await respuestaHttp.Content
                    .ReadFromJsonAsync<RespuestaSeries>();

            if (respuesta?.Series != null)
            {
                todasLasSeries.AddRange(respuesta.Series);
            }
        }

        return todasLasSeries
            .GroupBy(serie => serie.Id)
            .Select(grupo => grupo.First())
            .ToList();
    }

    public async Task<Trailer?> ObtenerTrailerAsync(
        int serieId)
    {
        string urlEspanol =
            $"https://api.themoviedb.org/3/tv/{serieId}/videos" +
            $"?api_key={ApiKey}&language=es-ES";

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
            $"https://api.themoviedb.org/3/tv/{serieId}/videos" +
            $"?api_key={ApiKey}&language=en-US";

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