namespace MAUINavegacion.Models;

public class PronosticoDia
{
    public string Dia { get; set; } = "";

    public double Temperatura { get; set; }

    public string Descripcion { get; set; } = "";

    public string Icono { get; set; } = "";

    public string IconoCompleto
    {
        get
        {
            return $"https://openweathermap.org/img/wn/{Icono}@2x.png";
        }
    }

    public string TemperaturaTexto
    {
        get
        {
            return $"{Temperatura:0}°C";
        }
    }
}
