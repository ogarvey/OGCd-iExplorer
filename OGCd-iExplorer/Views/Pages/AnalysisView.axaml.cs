using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using OGCdiExplorer.Services;
using OGCdiExplorer.ViewModels.Pages;
using OGCdiExplorer.ViewModels.Windows;
using OGCdiExplorer.Views.Windows;
using OGLibCDi.Enums;
using OGLibCDi.Models;
using ReactiveUI;

namespace OGCdiExplorer.Views.Pages;

public partial class AnalysisView : ReactiveUserControl<AnalysisViewModel>
{
    private byte[] _selectedBytes;
    private CdiVideoType _videoType;

    public AnalysisView()
    {
        InitializeComponent();
        this.WhenActivated(action =>
            action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
    }

    private void DoShowDialogAsync(InteractionContext<ImagePreviewViewModel,
        bool?> interaction)
    {
        var dialog = new ImagePreview();
        dialog.DataContext = interaction.Input;
        (dialog.DataContext as ImagePreviewViewModel).cdiVideoType = _videoType;
        dialog.Show((Window)TopLevel.GetTopLevel(this));
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        ((AnalysisViewModel)DataContext).ApplySectorTypeFilters();
    }

    private void MenuItemSave_OnClick(object? sender, RoutedEventArgs e)
    {
        var file = ((AnalysisViewModel)DataContext).SelectedCdiFile.FilePath;
        var sector = SectorList.SelectedItems[0] as CdiSector;
        var dataType = sector.SectorTypeString;
        var commonString = $"{file}_{sector.FileNumber}_{sector.Channel}_";
        var videoData = $"{sector.Coding.VideoString}_{sector.Coding.ResolutionString}";
        videoData += sector.Coding.IsOddLines ? "_Odd" : "_Even";
        videoData += sector.Coding.IsASCF ? "_ASCF" : "";
        var monoStereo = sector.Coding.IsMono ? "Mono" : "Stereo";
        var audioData = $"{monoStereo}_{sector.Coding.BitsPerSampleString}bit_{sector.Coding.SampleRateString}";
        var final = $"{sector.SectorIndex}";
        switch (dataType)
        {
            case "Video":
                final = $"{commonString}{videoData}_{final}";
                break;
            case "Audio":
                final = $"{commonString}{audioData}_{final}";
                break;
            case "Data":
                final = $"{commonString}{final}";
                break;
            default:
                final = $"{commonString}_{final}";
                break;
        }
        var dialog = new SaveFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "Binary", Extensions = { "bin" } });
        dialog.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
        dialog.InitialFileName = $"{final}.bin";
        var result = dialog.ShowAsync((Window)TopLevel.GetTopLevel(this));
        result.ContinueWith(task =>
        {
            if (task.Result != null)
            {
                File.WriteAllBytes(task.Result, _selectedBytes);
            }
        });
    }

    private void MenuItemImage_OnClick(object? sender, RoutedEventArgs e)
    {
        var sectors = SectorList.SelectedItems.Cast<CdiSector>().ToList();
        var sectorsToSave = new List<byte[]>();
        if (sectors.Count > 0)
        {
            foreach (var sector in sectors)
            {
                sectorsToSave.Add(sector.GetSectorData());
            }
            _selectedBytes = sectorsToSave.SelectMany(x => x).ToArray();
            ImageService.Instance.ImageBytes = _selectedBytes;
            _videoType = (CdiVideoType)sectors[0].Coding.Coding;
            ImageService.Instance.VideoType = _videoType;
            _videoType = (CdiVideoType)sectors[0].Coding.Coding;
        }
    }

    private void MenuItemPalette_OnClick(object? sender, RoutedEventArgs e)
    {
        var sectors = SectorList.SelectedItems.Cast<CdiSector>().ToList();
        var sectorsToSave = new List<byte[]>();
        if (sectors.Count > 0)
        {
            foreach (var sector in sectors)
            {
                sectorsToSave.Add(sector.GetSectorData());
            }
            _selectedBytes = sectorsToSave.SelectMany(x => x).ToArray();
            ImageService.Instance.PaletteBytes = _selectedBytes;
        }
    }
}