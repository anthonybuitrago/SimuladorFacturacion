namespace SimuladorFacturacion.WPF.Core.Interfaces;

/// <summary>
/// Contrato específico para el cálculo de descuentos.
/// Cumple con ISP (Interface Segregation Principle): Contrato mínimo y enfocado.
/// Cumple con OCP (Open/Closed Principle): Permite nuevas reglas de descuento sin modificar el código existente.
/// Cumple con LSP (Liskov Substitution Principle): Cualquier implementación puede sustituirse sin romper el cálculo.
/// </summary>
public interface ICalculadorDescuento
{
    string NombreTipo { get; }
    decimal Porcentaje { get; }
    decimal CalcularDescuento(decimal montoBase);
}
