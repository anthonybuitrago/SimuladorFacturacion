using System.Windows;

namespace SimuladorFacturacion.WPF.Views;

/// <summary>
/// Code-behind de MainWindow.xaml.
/// Siguiendo rigurosamente el patrón MVVM, este archivo NO CONTIENE lógica de negocio,
/// cálculos ni acceso a datos. Toda la interacción se delega al ViewModel por Data Binding.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Regla ergonómica: Al abrir la ventana, el cursor se coloca automáticamente en el campo de monto
        Loaded += (s, e) => TxtMontoBase.Focus();
    }
}
