using SimuladorFacturacion.WPF.Core.Interfaces;

namespace SimuladorFacturacion.WPF.Core.Strategies;

/// <summary>
/// Estrategia de descuento para clientes regulares (0% de descuento).
/// Cumple con SRP: Solo sabe calcular el descuento estándar.
/// </summary>
public class DescuentoEstandar : ICalculadorDescuento
{
    public string NombreTipo => "Cliente Estándar";
    public decimal Porcentaje => 0.00m;

    public decimal CalcularDescuento(decimal montoBase)
    {
        return 0.00m;
    }
}
