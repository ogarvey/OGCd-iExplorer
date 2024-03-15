using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using OGCdiExplorer.Extensions;
using OGCdiExplorer.Models;
using OGCdiExplorer.Services;
using OGCdiExplorer.ViewModels.Windows;
using OGLibCDi.Enums;
using OGLibCDi.Helpers;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Image = Avalonia.Controls.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace OGCdiExplorer.Views.Windows;

public partial class PaletteManagementView : Window
{
    private IStorageFile _imageFile;

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

    private async void ParsePalette_OnClick(object? sender, RoutedEventArgs e)
    {
        var paletteData = ((PaletteManagementViewModel)DataContext).PaletteData;
        var paletteOffset = ((PaletteManagementViewModel)DataContext).PaletteOffset;
        var paletteLength = ((PaletteManagementViewModel)DataContext).PaletteLength;

        if (paletteData.Length == 0 || paletteData.Length < paletteOffset || paletteLength == 0)
        {
            return;
        }

        if (paletteData.Length < paletteOffset + paletteLength)
        {
            paletteLength = paletteData.Length - paletteOffset;
            if (paletteLength <= 0)
            {
                return;
            }

            ((PaletteManagementViewModel)DataContext).PaletteLength = paletteLength;
        }

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
        ((PaletteManagementViewModel)DataContext).Palette = palette;
        PopulatePaletteImage(palette);
        if (ImageService.Instance?.ImageBytes?.Length > 0)
        {
            ((PaletteManagementViewModel)DataContext).ParseImage();
        }
    }

    private void PopulatePaletteImage(List<Color> palette)
    {
        var paletteBitmap = ColorHelper.CreateLabelledPalette(palette,24);

        var bitmapdata = paletteBitmap.LockBits(new Rectangle(0, 0, paletteBitmap.Width, paletteBitmap.Height),
            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
            bitmapdata.Scan0,
            new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
            new Avalonia.Vector(96, 96),
            bitmapdata.Stride);
        paletteBitmap.UnlockBits(bitmapdata);
        paletteBitmap.Dispose();
        ((PaletteManagementViewModel)DataContext).PaletteImage = bitmap1;
    }

    private void RotatePalette_OnClick(object? sender, RoutedEventArgs e)
    {
        ((PaletteManagementViewModel)DataContext).RotatePalette();
    }

    private async void ParseSectorsPalette_OnClick(object? sender, RoutedEventArgs e)
    {
        var paletteData = ImageService.Instance.PaletteBytes;
        if (paletteData is { Length: 0 })
        {
            return;
        }
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

        ((PaletteManagementViewModel)DataContext).Palette = palette;
        PopulatePaletteImage(palette);
        if (ImageService.Instance.ImageBytes.Length > 0)
        {
            ((PaletteManagementViewModel)DataContext).ParseImage();
        }
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
            case "NumImgWidth":
                ImageService.Instance.ImageWidth = (int)e.NewValue;
                if (ImageService.Instance?.ImageBytes?.Length > 0)
                {
                    ((PaletteManagementViewModel)DataContext).ParseImage();
                }
                break;
            case "NumImgHeight":
                ImageService.Instance.ImageHeight = (int)e.NewValue;
                if (ImageService.Instance?.ImageBytes?.Length > 0)
                {
                    ((PaletteManagementViewModel)DataContext).ParseImage();
                }
                break;
        }
    }

    private void AddRotation_OnClick(object? sender, RoutedEventArgs e)
    {
        var rotationStartIndex = ((PaletteManagementViewModel)DataContext).RotationStartIndex;
        var rotationEndIndex = ((PaletteManagementViewModel)DataContext).RotationEndIndex;
        var rotationPermutations = ((PaletteManagementViewModel)DataContext).RotationPermutations;
        var reverseRotation = ((PaletteManagementViewModel)DataContext).ReverseRotation;
        var rotationFrameSkip = ((PaletteManagementViewModel)DataContext).RotationFrameSkip;

        ((PaletteManagementViewModel)DataContext).PaletteRotations.Add(new PaletteRotation(rotationStartIndex,
            rotationEndIndex, rotationPermutations, rotationFrameSkip, reverseRotation));
    }

    private async void LoadImagePreview_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        var filter = new FilePickerFileType("Image")
        {
            Patterns = new List<string>() { "*.bin" }
        };
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Choose an image file",
            FileTypeFilter = new List<FilePickerFileType>() { filter }
        });

        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            _imageFile = files[0];
            using var stream = files[0].OpenReadAsync().Result;
            using var streamReader = new BinaryReader(stream);
            // Reads all the content of file as a text.
            var fileContent = streamReader.ReadAllBytes();
            ImageService.Instance.ImageBytes = fileContent;
        }
        
        if (ImageService.Instance?.ImageBytes?.Length > 0)
        {
            ((PaletteManagementViewModel)DataContext).ImageLoaded = true;
            ((PaletteManagementViewModel)DataContext).ParseImage();
        }
    }

    private void ImageFormat_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        switch (sender)
        {
            case RadioButton {Name: "RadImgRgb"}:
                ImageService.Instance.VideoType = CdiVideoType.RL7;
                break;
            case RadioButton {Name: "RadImgClut"}:
                ImageService.Instance.VideoType = CdiVideoType.CLUT7;
                break;
        }
    }

    private void RemoveRotation_OnClick(object? sender, RoutedEventArgs e)
    {
        var paletteRotations = ((PaletteManagementViewModel)DataContext).PaletteRotations;
        if (paletteRotations.Count > 0)
        {
            paletteRotations.RemoveAt(paletteRotations.Count - 1);
        }
    }

    private void ExportGif_OnClick(object? sender, RoutedEventArgs e)
    {
        ((PaletteManagementViewModel)DataContext).ExportImageWithRotations(_imageFile.Path.AbsolutePath);
    }
}