using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using OGCdiExplorer.Extensions;
using OGCdiExplorer.Services;
using OGCdiExplorer.ViewModels.Pages;
using OGLibCDi.Enums;
using OGLibCDi.Helpers;
using OGLibCDi.Models;
using SkiaSharp;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace OGCdiExplorer.Views.Pages;

public partial class PaletteManagementView : ReactiveUserControl<PaletteManagementViewModel>
{
    public PaletteManagementView()
    {
        InitializeComponent();
    }

    private async void LoadPalette_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var filter = new FilePickerFileType("Palette")
        {
            Patterns = new List<string>() { "*.bin" }
        };
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Choose a palette file",
            FileTypeFilter = new List<FilePickerFileType>() { filter }
        });
    
        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new BinaryReader(stream);
            // Reads all the content of file as a text.
            var fileContent = streamReader.ReadAllBytes();
            ((PaletteManagementViewModel)DataContext).PaletteData = fileContent;
        }
    }
    
    private async void SavePalette_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var filter = new FilePickerFileType("Palette")
        {
            Patterns = new List<string>() { "*.bin" }
        };
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Choose a palette file",
            FileTypeFilter = new List<FilePickerFileType>() { filter }
        });
    
        if (files.Count >= 1)
        {
            // Open writing stream from the first file.
            await using var stream = await files[0].OpenWriteAsync();
            using var streamWriter = new BinaryWriter(stream);
            // Writes all the content of file as a text.
            streamWriter.Write(((PaletteManagementViewModel)DataContext).PaletteData);
        }
    }
    
    private void ParsePalette_OnClick(object? sender, RoutedEventArgs e)
    {
        var paletteData = ((PaletteManagementViewModel)DataContext).PaletteData;
        var paletteOffset = ((PaletteManagementViewModel)DataContext).PaletteOffset;
        var paletteLength = ((PaletteManagementViewModel)DataContext).PaletteLength;
        
        var palette = new List<Color>();
        switch (ImageService.Instance.PaletteType)
        {
            case CdiPaletteType.RGB:
                palette = ColorHelper.ConvertBytesToRGB(paletteData.Skip(paletteOffset).Take(paletteLength)
                    .ToArray());
                break;
            case CdiPaletteType.Indexed:
                palette = ColorHelper.ReadPalette(paletteData.Skip(paletteOffset).Take(paletteLength).ToArray());
                break;
            case CdiPaletteType.ClutBanks:
                palette = ColorHelper.ReadClutBankPalettes(paletteData.Skip(paletteOffset).ToArray(),
                    (byte)paletteLength);
                break;
        }
        var paletteBitmap = ColorHelper.CreateLabelledPalette(palette);
        
        var bitmapdata = paletteBitmap.LockBits(new Rectangle(0, 0, paletteBitmap.Width, paletteBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
            bitmapdata.Scan0,
            new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
            new Avalonia.Vector(96, 96),
            bitmapdata.Stride);
        paletteBitmap.UnlockBits(bitmapdata);
        paletteBitmap.Dispose();
        ImgPalettePreview.Source = bitmap1;
    }

    private void ParseSectorsPalette_OnClick(object? sender, RoutedEventArgs e)
    {
        var paletteData = ImageService.Instance.PaletteBytes;
        
        var paletteOffset = ((PaletteManagementViewModel)DataContext).PaletteOffset;
        var paletteLength = ((PaletteManagementViewModel)DataContext).PaletteLength;
        
        var palette = new List<Color>();
        switch (ImageService.Instance.PaletteType)
        {
            case CdiPaletteType.RGB:
                palette = ColorHelper.ConvertBytesToRGB(paletteData.Skip(paletteOffset).Take(paletteLength)
                    .ToArray());
                break;
            case CdiPaletteType.Indexed:
                palette = ColorHelper.ReadPalette(paletteData.Skip(paletteOffset).Take(paletteLength).ToArray());
                break;
            case CdiPaletteType.ClutBanks:
                palette = ColorHelper.ReadClutBankPalettes(paletteData.Skip(paletteOffset).ToArray(),
                    (byte)paletteLength);
                break;
        }
        var paletteBitmap = ColorHelper.CreateLabelledPalette(palette);
        
        var bitmapdata = paletteBitmap.LockBits(new Rectangle(0, 0, paletteBitmap.Width, paletteBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
            bitmapdata.Scan0,
            new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
            new Avalonia.Vector(96, 96),
            bitmapdata.Stride);
        paletteBitmap.UnlockBits(bitmapdata);
        paletteBitmap.Dispose();
        ImgPalettePreview.Source = bitmap1;
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
                break;
            case "NumImgLength":
                ImageService.Instance.ImageLength = (int)e.NewValue;
                break;
            case "NumImgWidth":
                ImageService.Instance.ImageWidth = (int)e.NewValue;
                break; 
            case "NumImgHeight":
                ImageService.Instance.ImageHeight = (int)e.NewValue;
                break;
        }
    }
}