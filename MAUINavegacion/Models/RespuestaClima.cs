using System.Text.Json.Serialization;

namespace MAUINavegacion.Models;

public class RespuestaClima
{
    [JsonPropertyName("name")]
    public string Ciudad { get; set; } = "";

    [JsonPropertyName("main")]
    public MainClima Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherInfo> Weather { get; set; } = new();
}

public class MainClima
{
    [JsonPropertyName("temp")]
    public double Temperatura { get; set; }

    [JsonPropertyName("feels_like")]
    public double SensacionTermica { get; set; }

    [JsonPropertyName("humidity")]
    public int Humedad { get; set; }
}

public class WeatherInfo
{
    [JsonPropertyName("description")]
    public string Descripcion { get; set; } = "";

    [JsonPropertyName("icon")]
    public string Icono { get; set; } = "";
}
