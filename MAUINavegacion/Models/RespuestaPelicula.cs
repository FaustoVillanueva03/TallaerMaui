using System.Text.Json.Serialization;

namespace MAUINavegacion.Models;

public class RespuestaPeliculas
{
    [JsonPropertyName("results")]
    public List<Pelicula> Peliculas { get; set; } = new List<Pelicula>();
}