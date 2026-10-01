using SimuladorFacturacion.WPF.Core.Interfaces;

namespace SimuladorFacturacion.WPF.Core.Strategies;

/// <summary>
/// Estrategia de descuento para clientes VIP (15% de deducción).
/// Aplica redondeo bancario para garantizar exactitud financiera.
/// </summary>
public class DescuentoVIP : ICalculadorDescuento
{
    public string NombreTipo => "Cliente VIP";
    public decimal Porcentaje => 0.15m;

    public decimal CalcularDescuento(decimal montoBase)
    {
        if (montoBase <= 0) return 0.00m;
        decimal descuento = montoBase * Porcentaje;
        return Math.Round(descuento, 2, MidpointRounding.AwayFromZero);
    }
}
