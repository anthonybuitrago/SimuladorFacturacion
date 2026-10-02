namespace SimuladorFacturacion.WPF.Models;

/// <summary>
/// Modelo inmutable de datos que representa una Factura emitida.
/// Cumple con SRP (Single Responsibility Principle) y pureza de MVVM:
/// Solo almacena datos puros del dominio (números y fechas).
/// No contiene lógica de formateo de texto ni signos monetarios (estos se delegan a la vista XAML).
/// </summary>
public class Factura
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoCliente { get; set; } = string.Empty;
    public decimal MontoBase { get; set; }
    public decimal MontoDescuento { get; set; }
    public decimal MontoTotal { get; set; }
}
