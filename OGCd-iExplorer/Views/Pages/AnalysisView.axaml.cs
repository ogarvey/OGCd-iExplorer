using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using OGCdiExplorer.Controls.HexView.Services;
using OGCdiExplorer.Services;
using OGCdiExplorer.ViewModels.Pages;
using OGLibCDi.Enums;
using OGLibCDi.Models;

namespace OGCdiExplorer.Views.Pages;

public partial class AnalysisView : ReactiveUserControl<AnalysisViewModel>
{
    private byte[] _selectedBytes;
    private CdiVideoType _videoType;

    public AnalysisView()
    {
        InitializeComponent();
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
            ((AnalysisViewModel)DataContext).VideoType = _videoType;
            ImageService.Instance.VideoType = _videoType;
            if (_videoType == CdiVideoType.DYUV || ImageService.Instance.PaletteBytes?.Length > 0)
            {
                ((AnalysisViewModel)DataContext).PopulateImage();
                if (_videoType != CdiVideoType.DYUV)
                {
                    ((AnalysisViewModel)DataContext).PopulatePalette();
                }
            }
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
            ((AnalysisViewModel)DataContext).PopulatePalette();
            if (ImageService.Instance.ImageBytes?.Length > 0)
            {
                ((AnalysisViewModel)DataContext).PopulateImage();
            }
        }
    }

    private void SendToAudio_OnClick(object? sender, RoutedEventArgs e)
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
            ((AnalysisViewModel)DataContext).IsMono = sectors[0].Coding.IsMono;
            RadAud189.IsChecked = sectors[0].Coding.SamplingFrequencyValue == 18900;
            RadAud378.IsChecked = sectors[0].Coding.SamplingFrequencyValue == 37800;
            RadAud4Bps.IsChecked = sectors[0].Coding.BitsPerSample == 4;
            RadAud8Bps.IsChecked = sectors[0].Coding.BitsPerSample == 8;
            ((AnalysisViewModel)DataContext).AudioBytes = _selectedBytes;
            ((AnalysisViewModel)DataContext).PopulateAudio();
        }
    }

    private void MenuItemHex_OnClick(object? sender, RoutedEventArgs e)
    {
        var sectors = SectorList.SelectedItems.Cast<CdiSector>().ToList();
        var sectorsToSave = new List<byte[]>();
        if (sectors.Count > 0)
        {
            foreach (var sector in sectors)
            {
                sectorsToSave.Add(sector.GetSectorData());
            }
            var bytesToSend = sectorsToSave.SelectMany(x => x).ToArray();
            HexView.HexViewControl1.LineReader?.Dispose();
            HexView.HexViewControl1.LineReader = new MemoryMappedLineReader(bytesToSend);
            HexView.HexViewControl1.HexFormatter = new HexFormatter(bytesToSend.Length);
            HexView.HexViewControl1.InvalidateScrollable();
            HexView.PathTextBox.Text = ((AnalysisViewModel)DataContext).SelectedCdiFile.FilePath;
        }
    }
    private void UpdateFilter(object? sender, TextChangedEventArgs e)
    {
        ((AnalysisViewModel)DataContext)?.ApplySectorTypeFilters();
    }

    private void ApplyVideoTypeFilter(object? sender, EventArgs e)
    {
        ((AnalysisViewModel)DataContext).ApplySectorTypeFilters();
    }

    private void PaletteType_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (((RadioButton)sender).IsChecked == false) return;
        switch (((RadioButton)sender).Name)
        {
            case "RadPalRgb":
                ImageService.Instance.PaletteType = CdiPaletteType.RGB;
                break;
            case "RadPalIndexed":
                ImageService.Instance.PaletteType = CdiPaletteType.Indexed;
                break;
            case "RadPalClut":
                ImageService.Instance.PaletteType = CdiPaletteType.ClutBanks;
                break;
        }
    }

    private void NumericUpDown_OnValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        switch (((NumericUpDown)sender).Name)
        {
            case "NumPalOffset":
                ImageService.Instance.PaletteOffset = (int)e.NewValue;
                break;
            case "NumPalLength":
                ImageService.Instance.PaletteLength = (int)e.NewValue;
                break;
            case "NumImgOffset":
                ImageService.Instance.ImageOffset = (int)e.NewValue;
                if(ImageService.Instance.ImageBytes.Length > 0)
                    ((AnalysisViewModel)DataContext).PopulateImage();
                break;
            case "NumImgLength":
                ImageService.Instance.ImageLength = (int)e.NewValue;
                if(ImageService.Instance.ImageBytes.Length > 0)
                    ((AnalysisViewModel)DataContext).PopulateImage();
                break;
            case "NumImgWidth":
                ImageService.Instance.ImageWidth = (int)e.NewValue;
                ImgPreviewImage.Width = (int)e.NewValue *2;
                if(ImageService.Instance.ImageBytes.Length > 0)
                    ((AnalysisViewModel)DataContext).PopulateImage();
                break; 
            case "NumImgHeight":
                ImageService.Instance.ImageHeight = (int)e.NewValue;
                ImgPreviewImage.Height = (int)e.NewValue *2;
                if(ImageService.Instance.ImageBytes.Length > 0)
                    ((AnalysisViewModel)DataContext).PopulateImage();
                break;
        }
    }
}