using System.Text.Json.Serialization;

namespace MAUINavegacion.Models;

public class Pelicula
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Titulo { get; set; }

    [JsonPropertyName("overview")]
    public string Descripcion { get; set; }

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; }

    public string ImagenCompleta =>
        $"https://image.tmdb.org/t/p/w500{PosterPath}";
}