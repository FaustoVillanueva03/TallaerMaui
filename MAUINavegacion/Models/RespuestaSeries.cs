using System.Text.Json.Serialization;

namespace MAUINavegacion.Models;

public class RespuestaSeries
{
    [JsonPropertyName("page")]
    public int Pagina { get; set; }

    [JsonPropertyName("results")]
    public List<Serie> Series { get; set; } = new();

    [JsonPropertyName("total_pages")]
    public int TotalPaginas { get; set; }

    [JsonPropertyName("total_results")]
    public int TotalResultados { get; set; }
}