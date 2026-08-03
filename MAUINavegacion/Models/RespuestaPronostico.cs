using System.Text.Json.Serialization;

namespace MAUINavegacion.Models;

public class RespuestaPronostico
{
    [JsonPropertyName("list")]
    public List<PronosticoItem> Lista { get; set; } = new();
}

public class PronosticoItem
{
    [JsonPropertyName("dt_txt")]
    public string Fecha { get; set; } = "";

    [JsonPropertyName("main")]
    public MainPronostico Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherPronostico> Weather { get; set; } = new();
}

public class MainPronostico
{
    [JsonPropertyName("temp")]
    public double Temperatura { get; set; }
}

public class WeatherPronostico
{
    [JsonPropertyName("description")]
    public string Descripcion { get; set; } = "";

    [JsonPropertyName("icon")]
    public string Icono { get; set; } = "";
}
