using System.Text.Json.Serialization;

namespace MAUINavegacion.Models;

public class Serie
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? RutaImagen { get; set; }

    [JsonPropertyName("first_air_date")]
    public string? FechaEstreno { get; set; }

    [JsonPropertyName("vote_average")]
    public double Puntuacion { get; set; }

    public string ImagenCompleta
    {
        get
        {
            if (string.IsNullOrWhiteSpace(RutaImagen))
            {
                return "dotnet_bot.png";
            }

            return $"https://image.tmdb.org/t/p/w500{RutaImagen}";
        }
    }

    public string DescripcionCompleta
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Descripcion))
            {
                return "Esta serie no tiene una descripción disponible.";
            }

            return Descripcion;
        }
    }

    public string FechaTexto
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FechaEstreno))
            {
                return "Sin fecha";
            }

            if (DateTime.TryParse(FechaEstreno, out DateTime fecha))
            {
                return fecha.ToString("dd/MM/yyyy");
            }

            return FechaEstreno;
        }
    }

    public string PuntuacionTexto
    {
        get
        {
            return $"⭐ {Puntuacion:0.0}";
        }
    }
}