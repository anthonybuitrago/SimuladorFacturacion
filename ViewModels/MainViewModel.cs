using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimuladorFacturacion.WPF.Core.Interfaces;
using SimuladorFacturacion.WPF.Core.Services;
using SimuladorFacturacion.WPF.Core.Strategies;
using SimuladorFacturacion.WPF.Models;

namespace SimuladorFacturacion.WPF.ViewModels;

/// <summary>
/// ViewModel principal utilizando CommunityToolkit.Mvvm oficial de Microsoft.
/// Utiliza Source Generators ([ObservableProperty], [RelayCommand]) para eliminar el código repetitivo.
/// Cumple con DIP (Dependency Inversion): Admite inyección de dependencias por constructor.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IProcesadorFacturaService _procesador;
    private readonly IRepositorioFacturas _repositorio;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(FacturarCommand))]
    private string _montoBaseInput = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(FacturarCommand))]
    private OpcionCliente? _tipoSeleccionado;

    [ObservableProperty]
    private string _mensajeError = string.Empty;

    [ObservableProperty]
    private bool _tieneError;

    [ObservableProperty]
    private string _subtotalPrevio = "$ 0.00";

    [ObservableProperty]
    private string _ahorroPrevio = "$ 0.00 (0%)";

    [ObservableProperty]
    private string _totalPrevio = "$ 0.00";

    [ObservableProperty]
    private string _totalFacturasTexto = "Total facturas emitidas: 0";

    [ObservableProperty]
    private string _totalAcumuladoTexto = "$ 0.00";

    public ObservableCollection<OpcionCliente> TiposClientes { get; }
    public ObservableCollection<Factura> Facturas => _repositorio.ObtenerFacturas();

    /// <summary>
    /// Constructor por defecto (ensambla las dependencias estándar de forma directa, limpia y legible).
    /// </summary>
    public MainViewModel()
    {
        var repo = new RepositorioFacturasMemoria();
        _repositorio = repo;
        _procesador = new ProcesadorFacturaService(repo);

        TiposClientes = InicializarTiposClientes();
        _tipoSeleccionado = TiposClientes[0];
    }

    /// <summary>
    /// Constructor con inyección de dependencias (Principio D / IoC / Pruebas Unitarias).
    /// </summary>
    public MainViewModel(IProcesadorFacturaService procesador, IRepositorioFacturas repositorio)
    {
        _procesador = procesador ?? throw new ArgumentNullException(nameof(procesador));
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));

        TiposClientes = InicializarTiposClientes();
        _tipoSeleccionado = TiposClientes[0];
    }

    private static ObservableCollection<OpcionCliente> InicializarTiposClientes()
    {
        return new ObservableCollection<OpcionCliente>
        {
            new OpcionCliente
            {
                NombreVisible = "Cliente Estándar (0%)",
                Descripcion = "Sin descuento comercial aplicado",
                Estrategia = new DescuentoEstandar()
            },
            new OpcionCliente
            {
                NombreVisible = "Cliente VIP (15% de Descuento)",
                Descripcion = "Ahorro preferencial del 15%",
                Estrategia = new DescuentoVIP()
            },
            new OpcionCliente
            {
                NombreVisible = "Cliente Corporativo (25% de Descuento)",
                Descripcion = "Tarifa especial empresarial del 25%",
                Estrategia = new DescuentoCorporativo()
            }
        };
    }

    // Métodos parciales disparados por [ObservableProperty]
    partial void OnMontoBaseInputChanged(string value)
    {
        ValidarYActualizarPrevisualizacion();
    }

    partial void OnTipoSeleccionadoChanged(OpcionCliente? value)
    {
        ValidarYActualizarPrevisualizacion();
    }

    private void ValidarYActualizarPrevisualizacion()
    {
        if (string.IsNullOrWhiteSpace(MontoBaseInput))
        {
            MensajeError = string.Empty;
            TieneError = false;
            SubtotalPrevio = "$ 0.00";
            AhorroPrevio = "$ 0.00 (0%)";
            TotalPrevio = "$ 0.00";
            return;
        }

        string valorLimpio = MontoBaseInput.Trim().Replace(',', '.');

        if (!decimal.TryParse(valorLimpio, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal monto) || monto <= 0)
        {
            MensajeError = "⚠️ Ingrese un valor numérico válido mayor a 0 (ej. 1500.00)";
            TieneError = true;
            SubtotalPrevio = "$ 0.00";
            AhorroPrevio = "$ 0.00 (0%)";
            TotalPrevio = "$ 0.00";
            return;
        }

        // Si es válido, calculamos vista previa en vivo
        MensajeError = string.Empty;
        TieneError = false;

        decimal descuento = TipoSeleccionado?.Estrategia.CalcularDescuento(monto) ?? 0.00m;
        decimal total = monto - descuento;
        decimal porcentaje = (TipoSeleccionado?.Estrategia.Porcentaje ?? 0) * 100;

        SubtotalPrevio = $"$ {monto:N2}";
        AhorroPrevio = descuento > 0 ? $"- $ {descuento:N2} ({porcentaje:0}%)" : "$ 0.00 (0%)";
        TotalPrevio = $"$ {total:N2}";
    }

    private bool PuedeFacturar()
    {
        if (TieneError || string.IsNullOrWhiteSpace(MontoBaseInput) || TipoSeleccionado == null)
        {
            return false;
        }

        string valorLimpio = MontoBaseInput.Trim().Replace(',', '.');
        return decimal.TryParse(valorLimpio, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal monto) && monto > 0;
    }

    [RelayCommand(CanExecute = nameof(PuedeFacturar))]
    private void Facturar()
    {
        if (!PuedeFacturar() || TipoSeleccionado == null) return;

        string valorLimpio = MontoBaseInput.Trim().Replace(',', '.');
        if (decimal.TryParse(valorLimpio, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal monto))
        {
            _procesador.CrearFactura(monto, TipoSeleccionado.Estrategia);

            // Actualizamos el pie acumulado
            ActualizarResumenAcumulado();

            // Limpiamos el campo para la próxima operación
            MontoBaseInput = string.Empty;
        }
    }

    private void ActualizarResumenAcumulado()
    {
        decimal acumulado = 0;
        foreach (var fac in Facturas)
        {
            acumulado += fac.MontoTotal;
        }

        TotalFacturasTexto = $"Total facturas emitidas: {Facturas.Count}";
        TotalAcumuladoTexto = $"$ {acumulado:N2}";
    }
}
