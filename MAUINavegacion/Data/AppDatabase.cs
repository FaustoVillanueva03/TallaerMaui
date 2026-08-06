using MAUINavegacion.Models;
using SQLite;

namespace MAUINavegacion.Data;

public class AppDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public AppDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InicializarAsync()
    {
        await _database.CreateTableAsync<Usuario>();
        await _database.CreateTableAsync<Perfil>();
    }

    public async Task<int> RegistrarUsuarioAsync(Usuario usuario)
    {
        await InicializarAsync();

        return await _database.InsertAsync(usuario);
    }

    public async Task<Usuario?> ObtenerUsuarioPorNombreAsync(
        string nombreUsuario)
    {
        await InicializarAsync();

        return await _database
            .Table<Usuario>()
            .Where(usuario =>
                usuario.NombreUsuario == nombreUsuario)
            .FirstOrDefaultAsync();
    }

    public async Task<Usuario?> ValidarLoginAsync(
        string nombreUsuario,
        string contrasena)
    {
        await InicializarAsync();

        return await _database
            .Table<Usuario>()
            .Where(usuario =>
                usuario.NombreUsuario == nombreUsuario &&
                usuario.Contrasena == contrasena)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CrearPerfilAsync(Perfil perfil)
    {
        await InicializarAsync();

        return await _database.InsertAsync(perfil);
    }

    public async Task<List<Perfil>> ObtenerPerfilesAsync(
        int usuarioId)
    {
        await InicializarAsync();

        return await _database
            .Table<Perfil>()
            .Where(perfil =>
                perfil.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task<int> ActualizarPerfilAsync(Perfil perfil)
    {
        await InicializarAsync();

        return await _database.UpdateAsync(perfil);
    }

    public async Task<int> EliminarPerfilAsync(Perfil perfil)
    {
        await InicializarAsync();

        return await _database.DeleteAsync(perfil);
    }
    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(
    int usuarioId)
    {
        await InicializarAsync();

        return await _database
            .Table<Usuario>()
            .Where(usuario =>
                usuario.Id == usuarioId)
            .FirstOrDefaultAsync();
    }
}