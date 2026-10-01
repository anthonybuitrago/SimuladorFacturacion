# 🧾 Simulador de Facturación y Descuentos (WPF - MVVM)

Proyecto académico de **C# y .NET 8** desarrollado siguiendo rigurosamente el **Ciclo de Vida del Software** (Requisitos, Modelado de Datos, UI/UX, Arquitectura Técnica y Codificación) y enfocado en la aplicación limpia de los **principios SOLID (S-O-L-I)** y el patrón arquitectónico **MVVM**.

---

## 🎯 Mapeo de Principios SOLID en el Código

| Principio SOLID | Archivo(s) Clave | Justificación Técnica |
| :--- | :--- | :--- |
| **S - Single Responsibility** | `Models/Factura.cs`<br>`Core/Strategies/DescuentoVIP.cs`<br>`Views/MainWindow.xaml.cs` | Cada clase tiene una única razón de cambio. `Factura` solo almacena datos; las estrategias solo calculan deducciones; el Code-Behind solo arranca la ventana sin lógica de negocio. |
| **O - Open / Closed** | `Core/Interfaces/ICalculadorDescuento.cs`<br>`Core/Strategies/` | Abierto a la extensión y cerrado a la modificación. Para añadir un nuevo tipo de descuento (ej. *DescuentoEstudiante*), se crea una nueva clase que implemente la interfaz sin tocar ninguna línea del código existente. |
| **L - Liskov Substitution** | `Core/Services/ProcesadorFacturaService.cs` | El procesador opera con cualquier `ICalculadorDescuento` de forma intercambiable y transparente, sin requerir condicionales (`if/switch`) de tipo de cliente. |
| **I - Interface Segregation** | `ICalculadorDescuento.cs`<br>`IRepositorioFacturas.cs`<br>`IProcesadorFacturaService.cs` | Interfaces pequeñas, segregadas y directas. Ninguna clase se ve forzada a implementar métodos que no utiliza. |
| **D - Dependency Inversion** | `ViewModels/MainViewModel.cs` | El ViewModel recibe sus dependencias abstractas (`IProcesadorFacturaService`, `IRepositorioFacturas`) por constructor, desacoplándose de implementaciones fijas y permitiendo IoC / Unit Testing. |

---

## 🏗️ Estructura del Proyecto

```text
📁 SimuladorFacturacion/
├── 📄 SimuladorFacturacion.sln
├── 📄 SimuladorFacturacion.WPF.csproj
├── 📁 Models/
│   ├── Factura.cs
│   └── OpcionCliente.cs
├── 📁 Core/
│   ├── 📁 Interfaces/
│   │   ├── ICalculadorDescuento.cs
│   │   ├── IRepositorioFacturas.cs
│   │   └── IProcesadorFacturaService.cs
│   ├── 📁 Strategies/
│   │   ├── DescuentoEstandar.cs (0%)
│   │   ├── DescuentoVIP.cs (15%)
│   │   └── DescuentoCorporativo.cs (25%)
│   └── 📁 Services/
│       ├── RepositorioFacturasMemoria.cs
│       └── ProcesadorFacturaService.cs
├── 📁 ViewModels/
│   ├── 📁 Common/
│   │   ├── ViewModelBase.cs (INotifyPropertyChanged)
│   │   └── RelayCommand.cs (ICommand)
│   └── MainViewModel.cs
└── 📁 Views/
    ├── MainWindow.xaml (Diseño Windows 11 Fluent)
    └── MainWindow.xaml.cs (Code-Behind Limpio)
```

---

## 🚀 Cómo Ejecutar el Proyecto

1. Haz doble clic en el archivo **`SimuladorFacturacion.sln`** para abrirlo en **Visual Studio**.
2. Presiona **F5** (o haz clic en el botón verde *"Iniciar"*).
3. La ventana del simulador se abrirá lista para calcular y facturar.
