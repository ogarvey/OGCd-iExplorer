using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using OGCdiExplorer.Interfaces;

namespace OGCdiExplorer.Services;

public class WindowActivator : IWindowActivator
{
    private List<Window> _windows = new();
    
    public void ActivateWindow<T>() where T : Window
    {
        if (_windows.Any(x => x.GetType() == typeof(T)))
        {
            _windows.First(x => x.GetType() == typeof(T)).Activate();
            return;
        }

        switch (typeof(T).Name)
        {
            case "BoltFileParser":
                _windows.Add(new Views.Windows.BoltFileParser());
                break;
        }
    }

    public void CloseWindow<T>() where T : Window
    {
        throw new System.NotImplementedException();
    }
}