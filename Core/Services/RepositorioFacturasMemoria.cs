using System.Collections.ObjectModel;
using SimuladorFacturacion.WPF.Core.Interfaces;
using SimuladorFacturacion.WPF.Models;

namespace SimuladorFacturacion.WPF.Core.Services;

/// <summary>
/// Repositorio en memoria que mantiene la colección observable de facturas.
/// Al usar ObservableCollection, la vista WPF se entera automáticamente cuando se agrega una factura.
/// </summary>
public class RepositorioFacturasMemoria : IRepositorioFacturas
{
    private readonly ObservableCollection<Factura> _facturas = new();
    private int _contadorId = 1;

    public ObservableCollection<Factura> ObtenerFacturas() => _facturas;

    public void AgregarFactura(Factura factura)
    {
        ArgumentNullException.ThrowIfNull(factura);
        _facturas.Insert(0, factura); // Insertar al inicio para que la más reciente aparezca arriba
    }

    public int ObtenerSiguienteId() => _contadorId++;
}
