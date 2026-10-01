using System.Collections.ObjectModel;
using SimuladorFacturacion.WPF.Models;

namespace SimuladorFacturacion.WPF.Core.Interfaces;

/// <summary>
/// Contrato exclusivo para la persistencia y lectura de Facturas en memoria.
/// Cumple con ISP: No mezcla lógica de cálculo ni formateo visual.
/// </summary>
public interface IRepositorioFacturas
{
    ObservableCollection<Factura> ObtenerFacturas();
    void AgregarFactura(Factura factura);
    int ObtenerSiguienteId();
}
