using SQLite;

namespace MAUINavegacion.Models;

[Table("Perfiles")]
public class Perfil
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Relaciona el perfil con el usuario principal.
    [Indexed]
    public int UsuarioId { get; set; }

    [NotNull, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Direccion { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Telefono { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    public string RutaFoto { get; set; } = string.Empty;

    // Punto elegido en el mapa.
    public double Latitud { get; set; }

    public double Longitud { get; set; }

    // Preferencias de las secciones que puede ver.
    public bool MostrarPeliculas { get; set; } = true;

    public bool MostrarSeries { get; set; } = true;

    public bool MostrarClima { get; set; } = true;

    public bool MostrarCotizaciones { get; set; } = true;

    public bool MostrarMapa { get; set; } = true;

    // Indica si este perfil debe mostrar
    // contenido apto para menores de 18 años.
    public bool EsMenor18 { get; set; } = false;
}