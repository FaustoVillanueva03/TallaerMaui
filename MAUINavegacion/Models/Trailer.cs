using System.Text.Json.Serialization;

namespace MAUINavegacion.Models
{
    public class Trailer
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("site")]
        public string Sitio { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Tipo { get; set; } = string.Empty;

        [JsonPropertyName("official")]
        public bool EsOficial { get; set; }
    }
}