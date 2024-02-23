using Avalonia.Controls;

namespace OGCdiExplorer.Interfaces;

public interface IWindowActivator
{
    void ActivateWindow<T>() where T : Window;
    void CloseWindow<T>() where T : Window;
}