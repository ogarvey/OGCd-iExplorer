using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OGCdiExplorer.Extensions.Helpers;
using OGCdiExplorer.Services;
using OGLibCDi.Enums;
using ReactiveUI;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace OGCdiExplorer.ViewModels.Windows;

public class ImagePreviewViewModel : PageViewModel
{
    private CdiVideoType _cdiVideoType;

    public CdiVideoType cdiVideoType
    {
        get => _cdiVideoType;
        set => this.RaiseAndSetIfChanged(ref _cdiVideoType, value);
    }

    public string VideoType => $"Preview of {_cdiVideoType.ToString()} format image";

    public ImagePreviewViewModel(CdiVideoType videoType)
    {
        cdiVideoType = videoType;
        SetPalette(null, null);
        SetImage(null, null);
        ImageService.Instance.PaletteBytesChanged += SetPalette;

        ImageService.Instance.ImageBytesChanged += SetImage;
    }
    
    private void SetPalette(object? sender, EventArgs args)
    { 
        // var paletteBytes = ImageService.Instance.PaletteBytes.Skip(90).ToArray();
        // Colors = ColorHelper.ReadClutBankPalettes(paletteBytes, 2);
        // PaletteImg = paletteBytes != null
        //     ? ColorHelper.CreateLabelledPalette(Colors)
        //     : null;
        // ShowPalette = PaletteImg != null;
    }

    private void SetImage(object? sender, EventArgs args)
    {
        // var imageBytes = ImageService.Instance.ImageBytes;
        // var paletteBytes = ImageService.Instance.PaletteBytes;
        // Colors = ColorHelper.ReadClutBankPalettes(paletteBytes, 4);
        // PreviewImg = (imageBytes != null && Colors.Count > 0)
        //     ? ImageFormatHelper.GenerateRle7Image(Colors, imageBytes, 384, 240)
        //     : null;
    }
    public bool ShowPalette { get; set; }

    public bool ShowImage { get; set; }

    private List<Color> Colors { get; set; }

    private Bitmap? _previewImg;
    private Bitmap? _paletteImg;

    public Bitmap? PreviewImg
    {
        get => _previewImg;
        set => this.RaiseAndSetIfChanged(ref _previewImg, value);
    }

    public Bitmap? PaletteImg
    {
        get => _paletteImg;
        set => this.RaiseAndSetIfChanged(ref _paletteImg, value);
    }

    public override bool CanNavigateNext { get; protected set; }
    public override bool CanNavigatePrevious { get; protected set; }
}