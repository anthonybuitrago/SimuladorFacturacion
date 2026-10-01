using SimuladorFacturacion.WPF.Core.Interfaces;
using SimuladorFacturacion.WPF.Models;

namespace SimuladorFacturacion.WPF.Core.Services;

/// <summary>
/// Orquesta el cálculo matemático y el ensamblaje de la entidad Factura.
/// Cumple con LSP: Opera con cualquier ICalculadorDescuento sin importar su implementación concreta.
/// </summary>
public class ProcesadorFacturaService : IProcesadorFacturaService
{
    private readonly IRepositorioFacturas _repositorio;

    public ProcesadorFacturaService(IRepositorioFacturas repositorio)
    {
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
    }

    public Factura CrearFactura(decimal montoBase, ICalculadorDescuento estrategia)
    {
        ArgumentNullException.ThrowIfNull(estrategia);

        if (montoBase <= 0)
        {
            throw new ArgumentException("El monto base debe ser mayor a cero.", nameof(montoBase));
        }

        // Delegamos el cálculo a la estrategia (Polimorfismo / LSP)
        decimal descuento = estrategia.CalcularDescuento(montoBase);
        decimal total = montoBase - descuento;

        // Formato de nombre descriptivo para la tabla: Ej. "Cliente VIP (15%)"
        int pct = (int)(estrategia.Porcentaje * 100);
        string tipoClienteConPorcentaje = $"{estrategia.NombreTipo} ({pct}%)";

        var factura = new Factura
        {
            Id = _repositorio.ObtenerSiguienteId(),
            Fecha = DateTime.Now,
            TipoCliente = tipoClienteConPorcentaje,
            MontoBase = montoBase,
            MontoDescuento = descuento,
            MontoTotal = total
        };

        // Guardamos en el repositorio
        _repositorio.AgregarFactura(factura);

        return factura;
    }
}
