using SimuladorFacturacion.WPF.Core.Interfaces;

namespace SimuladorFacturacion.WPF.Models;

/// <summary>
/// Modelo de soporte para representar las opciones en el ComboBox de la interfaz.
/// Vincula el texto visible para el usuario con la estrategia de descuento correspondiente.
/// </summary>
public class OpcionCliente
{
    public string NombreVisible { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public ICalculadorDescuento Estrategia { get; set; } = null!;

    public override string ToString() => NombreVisible;
}
