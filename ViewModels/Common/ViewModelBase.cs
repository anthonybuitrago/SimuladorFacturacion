using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimuladorFacturacion.WPF.ViewModels.Common;

/// <summary>
/// Clase base para todos los ViewModels.
/// Implementa INotifyPropertyChanged para permitir el Data Binding reactivo bidireccional en WPF.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
