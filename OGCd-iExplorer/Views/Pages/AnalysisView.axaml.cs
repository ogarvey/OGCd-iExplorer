using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ManagedBass;
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
    private int _stream;

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
                sectorsToSave.Add(sector.GetSectorData(false, CdiSectorType.Audio));
            }
            _selectedBytes = sectorsToSave.SelectMany(x => x).ToArray();
            ((AnalysisViewModel)DataContext).IsMono = sectors[0].Coding.IsMono;
            RadAud189.IsChecked = sectors[0].Coding.SamplingFrequencyValue == 18900;
            RadAud378.IsChecked = sectors[0].Coding.SamplingFrequencyValue == 37800;
            RadAud4Bps.IsChecked = sectors[0].Coding.BitsPerSample == 0;
            RadAud8Bps.IsChecked = sectors[0].Coding.BitsPerSample == 1;
            ((AnalysisViewModel)DataContext).Frequency = sectors[0].Coding.SamplingFrequencyValue;
            ((AnalysisViewModel)DataContext).BitsPerSample = sectors[0].Coding.BitsPerSample == 0 ? 4 : 8;
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
    
    private void ApplySectorTypeFilter(object? sender, EventArgs e)
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
        switch ((sender as NumericUpDown)?.Name)
        {
            case "NumPalOffset":
                ImageService.Instance.PaletteOffset =  (int)(e.NewValue ?? 0.0m);
                break;
            case "NumPalLength":
                ImageService.Instance.PaletteLength =  (int)(e.NewValue ?? 384.0m);
                break;
            case "NumImgOffset":
                ImageService.Instance.ImageOffset = (int)(e.NewValue ?? 0.0m);
                if(ImageService.Instance?.ImageBytes?.Length > 0)
                    if (DataContext != null)
                        ((AnalysisViewModel)DataContext).PopulateImage();
                break;
            case "NumImgLength":
                ImageService.Instance.ImageLength =  (int)(e.NewValue ?? 92160.0m);
                if(ImageService.Instance?.ImageBytes?.Length > 0)
                    if (DataContext != null)
                        ((AnalysisViewModel)DataContext).PopulateImage();
                break;
            case "NumImgWidth":
                ImageService.Instance.ImageWidth =  (int)(e.NewValue ?? 384.0m);
                ImgPreviewImage.Width =  (int)(e.NewValue ?? 384.0m) *2;
                if(ImageService.Instance?.ImageBytes?.Length > 0)
                    if (DataContext != null)
                        ((AnalysisViewModel)DataContext).PopulateImage();
                break; 
            case "NumImgHeight":
                ImageService.Instance.ImageHeight =  (int)(e.NewValue ?? 240.0m);
                ImgPreviewImage.Height =  (int)(e.NewValue ?? 240.0m) *2;
                if(ImageService.Instance?.ImageBytes?.Length > 0)
                    if (DataContext != null)
                        ((AnalysisViewModel)DataContext).PopulateImage();
                break;
            case "NumImgInitialY":
                ImageService.Instance.InitialY =  (uint)(e.NewValue ?? 128.0m);
                if(ImageService.Instance?.ImageBytes?.Length > 0)
                    if (DataContext != null)
                        ((AnalysisViewModel)DataContext).PopulateImage();
                break;
            case "NumImgInitialU":
                ImageService.Instance.InitialU = (uint)(e.NewValue ?? 128.0m);
                if(ImageService.Instance?.ImageBytes?.Length > 0)
                    if (DataContext != null)
                        ((AnalysisViewModel)DataContext).PopulateImage();
                break;
            case "NumImgInitialV":
                ImageService.Instance.InitialV = (uint)(e.NewValue ?? 128.0m);
                if(ImageService.Instance?.ImageBytes?.Length > 0)
                    if (DataContext != null)
                        ((AnalysisViewModel)DataContext)?.PopulateImage();
                break;
        }
    }

    private void PlayAudio_OnClick(object? sender, RoutedEventArgs e)
    {
        if ((bool)RadAud189.IsChecked) 
            ((AnalysisViewModel)DataContext).Frequency = 18900;
        else 
            ((AnalysisViewModel)DataContext).Frequency = 37800;
        
        if ((bool)RadAud4Bps.IsChecked) 
            ((AnalysisViewModel)DataContext).BitsPerSample = 4;
        else 
            ((AnalysisViewModel)DataContext).BitsPerSample = 8;
        
        if ((bool)ChkMono.IsChecked) 
            ((AnalysisViewModel)DataContext).IsMono = true;
        else 
            ((AnalysisViewModel)DataContext).IsMono = false;
        
        ((AnalysisViewModel)DataContext).PopulateAudio();
        
        if (_stream == 0 && Bass.Init())
        {
            _stream = Bass.CreateStream(((AnalysisViewModel)DataContext).AMemoryStream.ToArray(), 0, ((AnalysisViewModel)DataContext).AMemoryStream.Length, BassFlags.Default);

            if (_stream != 0)
                Bass.ChannelPlay(_stream); // Play the stream
            
        } 
        else if (_stream != 0 && Bass.ChannelIsActive(_stream) == PlaybackState.Paused)
        {
            Bass.ChannelPlay(_stream); // Play the stream
        }
        else if (_stream != 0 && Bass.ChannelIsActive(_stream) == PlaybackState.Playing)
        {
            Bass.ChannelPause(_stream); // Play the stream
        }
        else
        {
            Debugger.Break();
        }
    }

    private void StopAudio_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_stream == 0) return;
        if (Bass.ChannelIsActive(_stream) != PlaybackState.Stopped)
            Bass.ChannelStop(_stream);
        Bass.StreamFree(_stream);
        Bass.Free();
        _stream = 0;
    }

    private void SaveAudio_OnClick(object? sender, RoutedEventArgs e)
    {
        var bytes =((AnalysisViewModel)DataContext).AMemoryStream.ToArray();
        var sector = SectorList.SelectedItems[0] as CdiSector;
        var file = ((AnalysisViewModel)DataContext).SelectedCdiFile.FileName;
        var bitsPerSample = sector.Coding.BitsPerSampleString;
        var sampleRate = sector.Coding.SampleRateString;
        var monoStereo = sector.Coding.IsMono ? "Mono" : "Stereo";
        var final = $"{file}_{sector.FileNumber}_{sector.Channel}_{monoStereo}_{bitsPerSample}_{sampleRate}_{sector.SectorIndex}";
        var dialog = new SaveFileDialog();
        dialog.Filters.Add(new FileDialogFilter() { Name = "WAV", Extensions = { "wav" } });
        dialog.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
        dialog.InitialFileName = $"{final}.wav";
        var result = dialog.ShowAsync((Window)TopLevel.GetTopLevel(this));
        result.ContinueWith(task =>
        {
            if (task.Result != null)
            {
                using (var fs = new FileStream(task.Result, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        });
    }

    private void ToFormatComboBox_OnDropDownClosed(object? sender, EventArgs e)
    {
        var format = ((ComboBox)sender).SelectedItem.ToString();
        switch (format)
        {
            case "CLUT7":
                ImageService.Instance.VideoType = CdiVideoType.CLUT7;
                break;
            case "RLE":
                ImageService.Instance.VideoType = CdiVideoType.RL7;
                break;
            case "DYUV":
                ImageService.Instance.VideoType = CdiVideoType.DYUV;
                break;
        }
        if(ImageService.Instance?.ImageBytes?.Length > 0)
            ((AnalysisViewModel)DataContext).PopulateImage();
    }

    private void SectorList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ((AnalysisViewModel)DataContext).SectorCount = SectorList.SelectedItems.Count;
    }
}