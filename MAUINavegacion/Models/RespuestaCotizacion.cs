using System.Text.Json.Serialization;

namespace MAUINavegacion.Models;

public class RespuestaCotizacion
{
    [JsonPropertyName("conversion_rates")]
    public ConversionRates ConversionRates { get; set; } = new();
}

public class ConversionRates
{
    [JsonPropertyName("USD")]
    public double Dolar { get; set; }

    [JsonPropertyName("EUR")]
    public double Euro { get; set; }

    [JsonPropertyName("BRL")]
    public double Real { get; set; }
}
