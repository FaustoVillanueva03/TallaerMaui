using System.Text.Json.Serialization;

namespace MAUINavegacion.Models
{
    public class RespuestaTrailers
    {
        [JsonPropertyName("results")]
        public List<Trailer> Trailers { get; set; } = new();
    }
}