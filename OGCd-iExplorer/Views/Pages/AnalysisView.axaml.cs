using System;
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
        var sectors = SectorList.SelectedItems.Cast<CdiSector>().ToList();
        var sectorsToSave = new List<byte[]>();
        var saveAsType = ((MenuItem)sender).Header.ToString().Replace("Save As ", "");
        
        if (sectors.Count > 0)
        {
            if (saveAsType != "Save Selected Sectors")
            {
                var sectorType = (CdiSectorType)Enum.Parse(typeof(CdiSectorType), saveAsType);
                foreach (var sector in sectors)
                {
                    sectorsToSave.Add(sector.GetSectorData(false, sectorType));
                }
                _selectedBytes = sectorsToSave.SelectMany(x => x).ToArray();
            }
            else
            {
                foreach (var sector in sectors)
                {
                    sectorsToSave.Add(sector.GetSectorData());
                }
                _selectedBytes = sectorsToSave.SelectMany(x => x).ToArray();
            }
                
        }
        var file = ((AnalysisViewModel)DataContext).SelectedCdiFile.FilePath;
        var sectorInfo = SectorList.SelectedItems[0] as CdiSector;
        var dataType = sectorInfo.SectorTypeString;
        var commonString = $"{file}_{sectorInfo.FileNumber}_{sectorInfo.Channel}_";
        var videoData = $"{sectorInfo.Coding.VideoString}_{sectorInfo.Coding.ResolutionString}";
        videoData += sectorInfo.Coding.IsOddLines ? "_Odd" : "_Even";
        videoData += sectorInfo.Coding.IsASCF ? "_ASCF" : "";
        var monoStereo = sectorInfo.Coding.IsMono ? "Mono" : "Stereo";
        var audioData = $"{monoStereo}_{sectorInfo.Coding.BitsPerSampleString}bit_{sectorInfo.Coding.SampleRateString}";
        var final = $"{sectorInfo.SectorIndex}";
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

    private void UpdateFilter(object? sender, TextChangedEventArgs e)
    {
        ((AnalysisViewModel)DataContext).ApplySectorTypeFilters();
    }

    private void ApplyVideoTypeFilter(object? sender, EventArgs e)
    {
        ((AnalysisViewModel)DataContext).ApplySectorTypeFilters();
    }
}