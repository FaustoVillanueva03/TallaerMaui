namespace MAUINavegacion.Models;

public class CarritoItem
{
    public int IdContenido { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Imagen { get; set; } = string.Empty;

    public string TipoContenido { get; set; } = string.Empty;

    public string Modalidad { get; set; } = string.Empty;

    public string Moneda { get; set; } = "UYU";

    public double Precio { get; set; }

    public double PrecioUYU { get; set; }

    public string ModalidadTexto
    {
        get
        {
            return $"{TipoContenido} · {Modalidad}";
        }
    }

    public string PrecioTexto
    {
        get
        {
            return Moneda switch
            {
                "USD" => $"USD {Precio:0.00}",
                "EUR" => $"EUR {Precio:0.00}",
                "BRL" => $"BRL {Precio:0.00}",
                _ => $"$ {Precio:0.00} UYU"
            };
        }
    }
}