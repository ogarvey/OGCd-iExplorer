using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using OGCdiExplorer.Extensions;
using OGCdiExplorer.Interfaces;
using OGCdiExplorer.Services;
using OGCdiExplorer.Views.Windows;
using OGLibCDi.Models;
using ReactiveUI;

namespace OGCdiExplorer.Controls;

public partial class MenuBar : UserControl
{ 

    public MenuBar()
    {
        InitializeComponent();
    }
    
    private async void MenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Choose a CD-i file",
        });
    
        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            try
            {
                await using var stream = await files[0].OpenReadAsync();
                using var streamReader = new BinaryReader(stream);
                // Reads all the content of file as a text.
                var fileContent = streamReader.ReadAllBytes();
                CdiFileService.Instance.UpdateCdiFile(new CdiFile(files[0].Path.LocalPath, fileContent));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}