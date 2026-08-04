using MAUINavegacion.Models;

namespace MAUINavegacion.Services;

public class CarritoService
{
    private static readonly List<CarritoItem> _items = new();

    public List<CarritoItem> ObtenerItems()
    {
        return _items.ToList();
    }

    public bool Agregar(CarritoItem nuevoItem)
    {
        CarritoItem? itemExistente =
            _items.FirstOrDefault(item =>
                item.IdContenido ==
                    nuevoItem.IdContenido &&
                item.TipoContenido ==
                    nuevoItem.TipoContenido);

        if (itemExistente != null)
        {
            itemExistente.Modalidad =
                nuevoItem.Modalidad;

            itemExistente.Moneda =
                nuevoItem.Moneda;

            itemExistente.Precio =
                nuevoItem.Precio;

            itemExistente.PrecioUYU =
                nuevoItem.PrecioUYU;

            return false;
        }

        _items.Add(nuevoItem);

        return true;
    }

    public void Eliminar(CarritoItem item)
    {
        CarritoItem? itemGuardado =
            _items.FirstOrDefault(x =>
                x.IdContenido ==
                    item.IdContenido &&
                x.TipoContenido ==
                    item.TipoContenido);

        if (itemGuardado != null)
        {
            _items.Remove(itemGuardado);
        }
    }

    public void Vaciar()
    {
        _items.Clear();
    }

    public int ObtenerCantidad()
    {
        return _items.Count;
    }

    public double ObtenerTotalUYU()
    {
        return _items.Sum(item =>
            item.PrecioUYU);
    }
}