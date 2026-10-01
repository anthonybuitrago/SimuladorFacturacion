using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using SimuladorFacturacion.WPF.Core.Interfaces;
using SimuladorFacturacion.WPF.Core.Services;
using SimuladorFacturacion.WPF.Core.Strategies;
using SimuladorFacturacion.WPF.Models;
using SimuladorFacturacion.WPF.ViewModels.Common;

namespace SimuladorFacturacion.WPF.ViewModels;

/// <summary>
/// ViewModel principal de la aplicación.
/// Orquesta el estado de la vista, validaciones en tiempo real y comandos de usuario.
/// Cumple con DIP (Dependency Inversion): Recibe sus dependencias abstractas por constructor.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly IProcesadorFacturaService _procesador;
    private readonly IRepositorioFacturas _repositorio;

    private string _montoBaseInput = string.Empty;
    private string _mensajeError = string.Empty;
    private bool _tieneError;
    private OpcionCliente? _tipoSeleccionado;

    private string _subtotalPrevio = "$ 0.00";
    private string _ahorroPrevio = "$ 0.00 (0%)";
    private string _totalPrevio = "$ 0.00";

    private string _totalFacturasTexto = "Total facturas emitidas: 0";
    private string _totalAcumuladoTexto = "$ 0.00";

    public ObservableCollection<OpcionCliente> TiposClientes { get; }
    public ObservableCollection<Factura> Facturas => _repositorio.ObtenerFacturas();

    public ICommand FacturarCommand { get; }

    /// <summary>
    /// Constructor por defecto (para XAML / diseñador).
    /// </summary>
    public MainViewModel() : this(CrearServiciosPorDefecto())
    {
    }

    private static (IProcesadorFacturaService procesador, IRepositorioFacturas repo) CrearServiciosPorDefecto()
    {
        var repo = new RepositorioFacturasMemoria();
        var procesador = new ProcesadorFacturaService(repo);
        return (procesador, repo);
    }

    private MainViewModel((IProcesadorFacturaService procesador, IRepositorioFacturas repo) servicios)
        : this(servicios.procesador, servicios.repo)
    {
    }

    /// <summary>
    /// Constructor con inyección de dependencias (Principio D / IoC).
    /// </summary>
    public MainViewModel(IProcesadorFacturaService procesador, IRepositorioFacturas repositorio)
    {
        _procesador = procesador ?? throw new ArgumentNullException(nameof(procesador));
        _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));

        // Inicializamos las opciones de clientes con los nombres idénticos al boceto
        TiposClientes = new ObservableCollection<OpcionCliente>
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

        _tipoSeleccionado = TiposClientes[0];

        FacturarCommand = new RelayCommand(EjecutarFacturar, PuedeFacturar);
    }

    public string MontoBaseInput
    {
        get => _montoBaseInput;
        set
        {
            if (SetProperty(ref _montoBaseInput, value))
            {
                ValidarYActualizarPrevisualizacion();
                (FacturarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public OpcionCliente? TipoSeleccionado
    {
        get => _tipoSeleccionado;
        set
        {
            if (SetProperty(ref _tipoSeleccionado, value))
            {
                ValidarYActualizarPrevisualizacion();
                (FacturarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string MensajeError
    {
        get => _mensajeError;
        private set => SetProperty(ref _mensajeError, value);
    }

    public bool TieneError
    {
        get => _tieneError;
        private set => SetProperty(ref _tieneError, value);
    }

    public string SubtotalPrevio
    {
        get => _subtotalPrevio;
        private set => SetProperty(ref _subtotalPrevio, value);
    }

    public string AhorroPrevio
    {
        get => _ahorroPrevio;
        private set => SetProperty(ref _ahorroPrevio, value);
    }

    public string TotalPrevio
    {
        get => _totalPrevio;
        private set => SetProperty(ref _totalPrevio, value);
    }

    public string TotalFacturasTexto
    {
        get => _totalFacturasTexto;
        private set => SetProperty(ref _totalFacturasTexto, value);
    }

    public string TotalAcumuladoTexto
    {
        get => _totalAcumuladoTexto;
        private set => SetProperty(ref _totalAcumuladoTexto, value);
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

        // Si es válido, limpiamos error y calculamos vista previa en vivo
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

    private void EjecutarFacturar()
    {
        if (!PuedeFacturar() || TipoSeleccionado == null) return;

        string valorLimpio = MontoBaseInput.Trim().Replace(',', '.');
        if (decimal.TryParse(valorLimpio, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal monto))
        {
            _procesador.CrearFactura(monto, TipoSeleccionado.Estrategia);

            // Actualizamos la barra de estado inferior
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
