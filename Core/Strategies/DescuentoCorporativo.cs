using SimuladorFacturacion.WPF.Core.Interfaces;

namespace SimuladorFacturacion.WPF.Core.Strategies;

/// <summary>
/// Estrategia de descuento para clientes corporativos (25% de deducción).
/// </summary>
public class DescuentoCorporativo : ICalculadorDescuento
{
    public string NombreTipo => "Cliente Corporativo";
    public decimal Porcentaje => 0.25m;

    public decimal CalcularDescuento(decimal montoBase)
    {
        if (montoBase <= 0) return 0.00m;
        decimal descuento = montoBase * Porcentaje;
        return Math.Round(descuento, 2, MidpointRounding.AwayFromZero);
    }
}
