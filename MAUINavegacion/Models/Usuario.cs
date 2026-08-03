using SQLite;

namespace MAUINavegacion.Models;

[Table("Usuarios")]
public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, NotNull, MaxLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    [NotNull, MaxLength(100)]
    public string Contrasena { get; set; } = string.Empty;

    [NotNull, MaxLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Direccion { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Telefono { get; set; } = string.Empty;

    [Unique, NotNull, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    public string RutaFoto { get; set; } = string.Empty;
}