using SimuladorFacturacion.WPF.Models;

namespace SimuladorFacturacion.WPF.Core.Interfaces;

/// <summary>
/// Contrato para el servicio de procesamiento y emisión de facturas.
/// Cumple con DIP (Dependency Inversion Principle): Expone la abstracción del servicio de facturación.
/// </summary>
public interface IProcesadorFacturaService
{
    Factura CrearFactura(decimal montoBase, ICalculadorDescuento estrategia);
}
