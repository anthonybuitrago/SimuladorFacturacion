namespace SimuladorFacturacion.WPF.Models;

/// <summary>
/// Modelo inmutable de datos que representa una Factura emitida.
/// Cumple con SRP (Single Responsibility Principle): Solo almacena y transporta datos.
/// </summary>
public class Factura
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoCliente { get; set; } = string.Empty;
    public decimal MontoBase { get; set; }
    public decimal MontoDescuento { get; set; }
    public decimal MontoTotal { get; set; }

    // Propiedades formateadas para coincidir exactamente con el boceto visual
    public string IdFormateado => $"#{Id:D3}";
    public string FechaFormateada => Fecha.ToString("yyyy-MM-dd HH:mm");
    public string MontoBaseFormateado => $"$ {MontoBase:N2}";
    public string MontoDescuentoFormateado => MontoDescuento > 0 ? $"- $ {MontoDescuento:N2}" : "$ 0.00";
    public string MontoTotalFormateado => $"$ {MontoTotal:N2}";
}
