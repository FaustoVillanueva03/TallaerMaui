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

    // PRECIOS EN PESOS URUGUAYOS

    [JsonIgnore]
    public double PrecioAlquilerUYU { get; set; }

    [JsonIgnore]
    public double PrecioCompraUYU { get; set; }

    // PRECIOS DE ALQUILER CONVERTIDOS

    [JsonIgnore]
    public double PrecioAlquilerUSD { get; set; }

    [JsonIgnore]
    public double PrecioAlquilerEUR { get; set; }

    [JsonIgnore]
    public double PrecioAlquilerBRL { get; set; }

    // PRECIOS DE COMPRA CONVERTIDOS

    [JsonIgnore]
    public double PrecioCompraUSD { get; set; }

    [JsonIgnore]
    public double PrecioCompraEUR { get; set; }

    [JsonIgnore]
    public double PrecioCompraBRL { get; set; }

    /*
     * Propiedades anteriores.
     * Se mantienen para que el código existente siga compilando.
     * Representan el precio de alquiler.
     */

    [JsonIgnore]
    public double PrecioUYU
    {
        get => PrecioAlquilerUYU;
        set => PrecioAlquilerUYU = value;
    }

    [JsonIgnore]
    public double PrecioUSD
    {
        get => PrecioAlquilerUSD;
        set => PrecioAlquilerUSD = value;
    }

    [JsonIgnore]
    public double PrecioEUR
    {
        get => PrecioAlquilerEUR;
        set => PrecioAlquilerEUR = value;
    }

    [JsonIgnore]
    public double PrecioBRL
    {
        get => PrecioAlquilerBRL;
        set => PrecioAlquilerBRL = value;
    }

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

            if (DateTime.TryParse(
                FechaEstreno,
                out DateTime fecha))
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

    // TEXTOS DEL ALQUILER

    public string PrecioAlquilerUYUTexto
    {
        get
        {
            return $"$ {PrecioAlquilerUYU:0.00} UYU";
        }
    }

    public string PrecioAlquilerUSDTexto
    {
        get
        {
            return $"USD {PrecioAlquilerUSD:0.00}";
        }
    }

    public string PrecioAlquilerEURTexto
    {
        get
        {
            return $"EUR {PrecioAlquilerEUR:0.00}";
        }
    }

    public string PrecioAlquilerBRLTexto
    {
        get
        {
            return $"BRL {PrecioAlquilerBRL:0.00}";
        }
    }

    // TEXTOS DE LA COMPRA

    public string PrecioCompraUYUTexto
    {
        get
        {
            return $"$ {PrecioCompraUYU:0.00} UYU";
        }
    }

    public string PrecioCompraUSDTexto
    {
        get
        {
            return $"USD {PrecioCompraUSD:0.00}";
        }
    }

    public string PrecioCompraEURTexto
    {
        get
        {
            return $"EUR {PrecioCompraEUR:0.00}";
        }
    }

    public string PrecioCompraBRLTexto
    {
        get
        {
            return $"BRL {PrecioCompraBRL:0.00}";
        }
    }

    // TEXTOS ANTERIORES: REPRESENTAN ALQUILER

    public string PrecioUYUTexto
    {
        get
        {
            return PrecioAlquilerUYUTexto;
        }
    }

    public string PrecioUSDTexto
    {
        get
        {
            return PrecioAlquilerUSDTexto;
        }
    }

    public string PrecioEURTexto
    {
        get
        {
            return PrecioAlquilerEURTexto;
        }
    }

    public string PrecioBRLTexto
    {
        get
        {
            return PrecioAlquilerBRLTexto;
        }
    }
}