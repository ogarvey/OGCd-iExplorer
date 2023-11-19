using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using OGCdiExplorer.Extensions.Helpers;
using OGCdiExplorer.Services;

namespace OGCdiExplorer.ViewModels.Controls;


public readonly record struct ColorBgra(byte B, byte G, byte R, byte A);

public partial class SkiaBitmapViewModel : ObservableObject
{
    [ObservableProperty] private int _framesPerSecond;

    public int ImageWidth { get; } = 1920;
    public int ImageHeight { get; } = 1080;

    public ColorBgra[] Image => _image;
    private ColorBgra[] _image;

    public Action? OnImageChanged { get; set; }

    private ColorBgra[] _palette;

    public SkiaBitmapViewModel()
    {
        _image = new ColorBgra[ImageWidth * ImageHeight];
        _palette = new ColorBgra[256];
        var paletteBytes = ImageService.Instance.PaletteBytes.Skip(90).ToArray();
        var colors = ColorHelper.ReadClutBankPalettes(paletteBytes, 2);
        var rleBytes = ImageService.Instance.ImageBytes;
        var decodedRle = ImageFormatHelper.Rle7(rleBytes, 384, 240);
        var imageBytes = ImageFormatHelper.CreateImageData(decodedRle, colors, 384, 240);

        OnImageChanged?.Invoke();
    }

    private ColorBgra MakeRgba(byte r, byte g, byte b, byte a) => new ColorBgra(b, g, r, a);

    private ColorBgra MakeRandomColor()
    {
        return new ColorBgra((byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), 255);
    }
}