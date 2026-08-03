using System.Text.Json.Serialization;

namespace MAUINavegacion.Models
{
    public class Pelicula
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Titulo { get; set; } = string.Empty;

        [JsonPropertyName("overview")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public string FechaEstreno { get; set; } = string.Empty;

        [JsonPropertyName("vote_average")]
        public double Puntuacion { get; set; }

        public double PrecioUYU { get; set; }

        public string ImagenCompleta
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PosterPath))
                    return "dotnet_bot.png";

                return $"https://image.tmdb.org/t/p/w500{PosterPath}";
            }
        }

        public string PuntuacionTexto
        {
            get
            {
                return $"⭐ {Puntuacion:0.0}";
            }
        }

        public string FechaTexto
        {
            get
            {
                if (DateTime.TryParse(FechaEstreno, out DateTime fecha))
                    return fecha.ToString("dd/MM/yyyy");

                return "Fecha no disponible";
            }
        }

        public string DescripcionCompleta
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Descripcion))
                    return "Esta película no tiene una descripción disponible.";

                return Descripcion;
            }
        }
    }
}