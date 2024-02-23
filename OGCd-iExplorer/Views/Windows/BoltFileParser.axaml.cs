using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.Core;
using OGCdiExplorer.ViewModels.Windows;

namespace OGCdiExplorer.Views.Windows;

public partial class BoltFileParser : Window
{
    public BoltFileParser()
    {
        InitializeComponent();
        
        DropBorder.AddHandler(DragDrop.DropEvent, Drop);
        DropBorder.AddHandler(DragDrop.DragOverEvent, DragOver);
        DropBorder.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
    }

    private void DragLeave(object? sender, DragEventArgs e)
    {
        DropBorder.Background = new SolidColorBrush(Colors.Gray);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        DropBorder.Background = new SolidColorBrush(Colors.DarkCyan);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.Files)) return;
        var files = e.Data.GetFiles();
        var enumerable = files?.ToList();
        if (IEnumerableExtensions.Count(enumerable) > 0)
        {
            if (files != null)
            {
                var file = Enumerable.First(enumerable);
                var bytes = File.ReadAllBytes(file.Path.LocalPath);
                if (bytes.Length > 0)
                {
                    DropBorder.Background = new SolidColorBrush(Colors.Green);
                    ((BoltFileParserViewModel)DataContext).FileBytes = bytes;
                    ((BoltFileParserViewModel)DataContext).ShowFileReceiver = false;
                    return;
                }
            }
            
            DropBorder.Background = new SolidColorBrush(Colors.Gray);
        }
        DropBorder.Background = new SolidColorBrush(Colors.Gray);
    }

    private void WindowBase_OnResized(object? sender, WindowResizedEventArgs e)
    {
        var window = (Window)sender!;
        var height = window.Height;

        OffsetsDataGrid.Height = height - 50;
    }
}