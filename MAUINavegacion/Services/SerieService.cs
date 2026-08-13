using System.Net.Http.Json;
using System.Text.Json;
using MAUINavegacion.Models;

namespace MAUINavegacion.Services;

public class SerieService
{
    private readonly HttpClient _httpClient;

    private const string ApiKey =
        "d11a00025e66dbb1f44ffcc5aef97948";

    public SerieService()
    {
        _httpClient =
            new HttpClient();
    }

    private bool EsPerfilMenor()
    {
        return Preferences.Default.Get(
            "EsMenor18",
            false);
    }

    public async Task<List<Serie>> ObtenerSeriesAsync()
    {
        List<Serie> todasLasSeries =
            new();

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
                await respuestaHttp.Content
                    .ReadAsStringAsync();

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
                todasLasSeries.AddRange(
                    respuesta.Series);
            }
        }

        List<Serie> series =
            todasLasSeries
                .GroupBy(serie => serie.Id)
                .Select(grupo => grupo.First())
                .ToList();

        if (EsPerfilMenor())
        {
            series =
                await FiltrarSeriesAptasAsync(
                    series);
        }

        AsignarPrecios(series);

        return series;
    }

    public async Task<List<Serie>> BuscarSeriesAsync(
        string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return new List<Serie>();
        }

        string textoCodificado =
            Uri.EscapeDataString(
                texto.Trim());

        string url =
            $"https://api.themoviedb.org/3/search/tv" +
            $"?api_key={ApiKey}" +
            $"&language=es-ES" +
            $"&include_adult=false" +
            $"&query={textoCodificado}" +
            $"&page=1";

        HttpResponseMessage respuestaHttp =
            await _httpClient.GetAsync(url);

        string contenido =
            await respuestaHttp.Content
                .ReadAsStringAsync();

        if (!respuestaHttp.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Error TMDB {(int)respuestaHttp.StatusCode}: " +
                contenido);
        }

        RespuestaSeries? respuesta =
            await respuestaHttp.Content
                .ReadFromJsonAsync<RespuestaSeries>();

        if (respuesta?.Series == null)
        {
            return new List<Serie>();
        }

        List<Serie> series =
            respuesta.Series
                .GroupBy(serie => serie.Id)
                .Select(grupo => grupo.First())
                .ToList();

        if (EsPerfilMenor())
        {
            series =
                await FiltrarSeriesAptasAsync(
                    series);
        }

        AsignarPrecios(series);

        return series;
    }

    public async Task<List<Serie>>
        ObtenerSeriesPorGeneroAsync(
            int generoId,
            int cantidad = 20)
    {
        string url =
            $"https://api.themoviedb.org/3/discover/tv" +
            $"?api_key={ApiKey}" +
            $"&language=es-ES" +
            $"&include_adult=false" +
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
                $"Error TMDB {(int)respuestaHttp.StatusCode}: " +
                contenido);
        }

        RespuestaSeries? respuesta =
            await respuestaHttp.Content
                .ReadFromJsonAsync<RespuestaSeries>();

        if (respuesta?.Series == null)
        {
            return new List<Serie>();
        }

        List<Serie> series =
            respuesta.Series
                .GroupBy(serie => serie.Id)
                .Select(grupo => grupo.First())
                .ToList();

        if (EsPerfilMenor())
        {
            series =
                await FiltrarSeriesAptasAsync(
                    series);
        }

        series =
            series
                .Take(cantidad)
                .ToList();

        AsignarPrecios(series);

        return series;
    }

    private async Task<List<Serie>>
        FiltrarSeriesAptasAsync(
            List<Serie> series)
    {
        List<Serie> aptas =
            new();

        // Evitamos hacer demasiadas llamadas
        // a TMDB de una sola vez.
        List<Serie> candidatas =
            series
                .Take(40)
                .ToList();

        foreach (Serie serie in candidatas)
        {
            bool esApta =
                await EsSerieAptaAsync(
                    serie.Id);

            if (esApta)
            {
                aptas.Add(
                    serie);
            }
        }

        return aptas;
    }

    private async Task<bool> EsSerieAptaAsync(
        int serieId)
    {
        try
        {
            string url =
                $"https://api.themoviedb.org/3/tv/" +
                $"{serieId}/content_ratings" +
                $"?api_key={ApiKey}";

            string json =
                await _httpClient
                    .GetStringAsync(url);

            using JsonDocument documento =
                JsonDocument.Parse(json);

            JsonElement raiz =
                documento.RootElement;

            if (!raiz.TryGetProperty(
                    "results",
                    out JsonElement resultados))
            {
                return false;
            }

            foreach (JsonElement resultado in resultados
                         .EnumerateArray())
            {
                string pais =
                    resultado.GetProperty(
                            "iso_3166_1")
                        .GetString() ??
                    string.Empty;

                if (pais != "US")
                {
                    continue;
                }

                string clasificacion =
                    resultado.GetProperty(
                            "rating")
                        .GetString() ??
                    string.Empty;

                return EsClasificacionSerieApta(
                    clasificacion);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static bool EsClasificacionSerieApta(
    string clasificacion)
    {
        string valor =
            clasificacion
                .Trim()
                .ToUpperInvariant();

        return valor == "TV-Y" ||
               valor == "TV-Y7" ||
               valor == "TV-G" ||
               valor == "TV-PG";
    }

    private static void AsignarPrecios(
        IEnumerable<Serie> series)
    {
        foreach (Serie serie in series)
        {
            serie.PrecioAlquilerUYU =
                CalcularPrecioAlquiler(
                    serie.Puntuacion);

            serie.PrecioCompraUYU =
                CalcularPrecioCompra(
                    serie.Puntuacion);
        }
    }

    private static double CalcularPrecioAlquiler(
        double puntuacion)
    {
        if (puntuacion >= 8)
        {
            return 420;
        }

        if (puntuacion >= 7)
        {
            return 350;
        }

        return 280;
    }

    private static double CalcularPrecioCompra(
        double puntuacion)
    {
        if (puntuacion >= 8)
        {
            return 1100;
        }

        if (puntuacion >= 7)
        {
            return 900;
        }

        return 700;
    }

    public async Task<Trailer?> ObtenerTrailerAsync(
        int serieId)
    {
        string urlEspanol =
            $"https://api.themoviedb.org/3/tv/" +
            $"{serieId}/videos" +
            $"?api_key={ApiKey}" +
            $"&language=es-ES";

        RespuestaTrailers? respuesta =
            await _httpClient
                .GetFromJsonAsync<RespuestaTrailers>(
                    urlEspanol);

        Trailer? trailer =
            ElegirTrailer(
                respuesta?.Trailers);

        if (trailer != null)
        {
            return trailer;
        }

        string urlIngles =
            $"https://api.themoviedb.org/3/tv/" +
            $"{serieId}/videos" +
            $"?api_key={ApiKey}" +
            $"&language=en-US";

        respuesta =
            await _httpClient
                .GetFromJsonAsync<RespuestaTrailers>(
                    urlIngles);

        return ElegirTrailer(
            respuesta?.Trailers);
    }

    private static Trailer? ElegirTrailer(
        List<Trailer>? videos)
    {
        if (videos == null)
        {
            return null;
        }

        List<Trailer> videosYoutube =
            videos
                .Where(video =>
                    !string.IsNullOrWhiteSpace(
                        video.Sitio) &&
                    video.Sitio.Equals(
                        "YouTube",
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

        return videosYoutube.FirstOrDefault(video =>
                   !string.IsNullOrWhiteSpace(
                       video.Tipo) &&
                   video.Tipo.Equals(
                       "Trailer",
                       StringComparison.OrdinalIgnoreCase) &&
                   video.EsOficial)

               ?? videosYoutube.FirstOrDefault(video =>
                   !string.IsNullOrWhiteSpace(
                       video.Tipo) &&
                   video.Tipo.Equals(
                       "Trailer",
                       StringComparison.OrdinalIgnoreCase))

               ?? videosYoutube.FirstOrDefault(video =>
                   !string.IsNullOrWhiteSpace(
                       video.Tipo) &&
                   video.Tipo.Equals(
                       "Teaser",
                       StringComparison.OrdinalIgnoreCase));
    }
}