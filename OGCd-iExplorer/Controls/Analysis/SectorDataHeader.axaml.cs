using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace OGCdiExplorer.Controls.Analysis;

public class SectorDataHeader : TemplatedControl
{
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent = RoutedEvent.Register<SectorDataHeader, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);
    
    public event EventHandler<RoutedEventArgs>? Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }
    
    public static readonly DirectProperty<SectorDataHeader, string?> SectorTypeProperty = AvaloniaProperty.RegisterDirect<SectorDataHeader, string?>(
        nameof(SectorType),
        o => o.SectorType,
        (o, v) => o.SectorType = v,
        defaultBindingMode: Avalonia.Data.BindingMode.OneWay,
        enableDataValidation: true
        );
    
    private string? _sectorType;
    public string? SectorType
    {
        get => _sectorType ?? "";
        set => SetAndRaise(SectorTypeProperty, ref _sectorType, value);
    }

    public static readonly DirectProperty<SectorDataHeader,  string?> SectorCountProperty = AvaloniaProperty.RegisterDirect<SectorDataHeader,  string?>(
        nameof(SectorCount),
        o => o.SectorCount,
        (o, v) => o.SectorCount = v,
        defaultBindingMode: Avalonia.Data.BindingMode.OneWay,
        enableDataValidation: true
    );
    
    private  string? _sectorCount;
    public  string? SectorCount
    {
        get => _sectorCount;
        set => SetAndRaise(SectorCountProperty, ref _sectorCount, value);
    }
}