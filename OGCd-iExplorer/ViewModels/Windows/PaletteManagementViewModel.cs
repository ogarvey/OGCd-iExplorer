using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OGCdiExplorer.Models;
using OGCdiExplorer.Services;
using OGLibCDi.Enums;
using OGLibCDi.Helpers;
using ReactiveUI;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace OGCdiExplorer.ViewModels.Windows;

public class PaletteManagementViewModel : PageViewModel
{
    public override bool CanNavigateNext { get; protected set; }
    public override bool CanNavigatePrevious { get; protected set; }

    private int _paletteOffset = 0;
    public int PaletteOffset
    {
        get => _paletteOffset;
        set => this.RaiseAndSetIfChanged(ref _paletteOffset, value);
    }
    
    private int _paletteLength = 384;
    public int PaletteLength
    {
        get => _paletteLength;
        set => this.RaiseAndSetIfChanged(ref _paletteLength, value);
    }
    
    private int _imageWidth = 320;
    public int ImageWidth
    {
        get => _imageWidth;
        set => this.RaiseAndSetIfChanged(ref _imageWidth, value);
    }
    
    private int _imageHeight = 160;
    public int ImageHeight
    {
        get => _imageHeight;
        set => this.RaiseAndSetIfChanged(ref _imageHeight, value);
    }
    
    private byte[] _paletteData = new byte[384];
    public byte[] PaletteData
    {
        get => _paletteData;
        set => this.RaiseAndSetIfChanged(ref _paletteData, value);
    }
    
    private int _rotationStartIndex = 0;
    public int RotationStartIndex
    {
        get => _rotationStartIndex;
        set => this.RaiseAndSetIfChanged(ref _rotationStartIndex, value);
    }
    
    private int _rotationEndIndex = 0;
    public int RotationEndIndex
    {
        get => _rotationEndIndex;
        set => this.RaiseAndSetIfChanged(ref _rotationEndIndex, value);
    }
    
    private int _rotationPermutations = 1;
    public int RotationPermutations
    {
        get => _rotationPermutations;
        set => this.RaiseAndSetIfChanged(ref _rotationPermutations, value);
    }
    
    private int _rotationCount = 1;
    public int RotationCount
    {
        get => _rotationCount;
        set => this.RaiseAndSetIfChanged(ref _rotationCount, value);
    }
    
    private bool _reverseRotation = false;
    public bool ReverseRotation
    {
        get => _reverseRotation;
        set => this.RaiseAndSetIfChanged(ref _reverseRotation, value);
    }
    
    private List<Color> _palette = new();
    public List<Color> Palette
    {
        get => _palette;
        set => this.RaiseAndSetIfChanged(ref _palette, value);
    }
    
    private Bitmap? _paletteImage;
    public Bitmap? PaletteImage
    {
        get => _paletteImage;
        set => this.RaiseAndSetIfChanged(ref _paletteImage, value);
    }
    
    private Bitmap? _testImage;
    public Bitmap? TestImage
    {
        get => _testImage;
        set => this.RaiseAndSetIfChanged(ref _testImage, value);
    }
    
    private ObservableCollection<PaletteRotation> _paletteRotations = new();
    public ObservableCollection<PaletteRotation> PaletteRotations
    {
        get => _paletteRotations;
        set => this.RaiseAndSetIfChanged(ref _paletteRotations, value);
    }

    private bool _imageLoaded;
    public bool ImageLoaded
    {
        get => _imageLoaded;
        set => this.RaiseAndSetIfChanged(ref _imageLoaded, value);
    }

    public void ParseImage()
    {
        System.Drawing.Bitmap img = new System.Drawing.Bitmap(ImageWidth, ImageHeight);
        if (ImageService.Instance.ImageBytes?.Length > 0)
        {
            switch (ImageService.Instance.VideoType)
            {
                case CdiVideoType.CLUT7:
                    img = ImageFormatHelper.GenerateClutImage(Palette, ImageService.Instance.ImageBytes, ImageWidth, ImageHeight);
                    break;
                case CdiVideoType.RL7:
                default:
                    img = new System.Drawing.Bitmap(ImageFormatHelper.GenerateRle7Image(Palette, ImageService.Instance.ImageBytes, ImageWidth, ImageHeight));
                    break;
            }
            
            var bitmapdata = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
                bitmapdata.Scan0,
                new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
                new Avalonia.Vector(96, 96),
                bitmapdata.Stride);
            img.UnlockBits(bitmapdata);
            img.Dispose();
            TestImage = bitmap1;
        }
    }

    public async Task RotatePalette()
    {
        var rotations =PaletteRotations;
        var repeat = false;
        var paletteData =PaletteData;
        var paletteOffset =PaletteOffset;
        var paletteLength =PaletteLength;
        var rotationCount =RotationCount;

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
            PaletteLength = paletteLength;
        }


        switch (ImageService.Instance.PaletteType)
        {
            case CdiPaletteType.RGB:
                Palette = ColorHelper.ConvertBytesToRGB(paletteData.Skip(paletteOffset).Take(paletteLength)
                    .ToArray());
                break;
            case CdiPaletteType.Indexed:
                Palette = ColorHelper.ReadPalette(paletteData.Skip(paletteOffset).Take(paletteLength).ToArray());
                break;
            case CdiPaletteType.ClutBanks:
                Palette = ColorHelper.ReadClutBankPalettes(paletteData.Skip(paletteOffset).ToArray(),
                    (byte)paletteLength);
                break;
        }

        for (int i = 0; i < rotationCount; i++)
        {
            if (i+1 == rotationCount && repeat) i = 0;
            foreach (var rotation in rotations)
            {
                if (rotation.reverseRotation)
                {
                    ColorHelper.ReverseRotateSubset(Palette, rotation.StartIndex, rotation.EndIndex,
                        rotation.Permutations);
                }
                else
                {
                    ColorHelper.RotateSubset(Palette, rotation.StartIndex, rotation.EndIndex,
                        rotation.Permutations);
                }
            }
            var paletteBitmap = ColorHelper.CreateLabelledPalette(Palette);
            
            var bitmapdata = paletteBitmap.LockBits(new Rectangle(0, 0, paletteBitmap.Width, paletteBitmap.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
                bitmapdata.Scan0,
                new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
                new Avalonia.Vector(96, 96),
                bitmapdata.Stride);
            paletteBitmap.UnlockBits(bitmapdata);
            paletteBitmap.Dispose();
            
            PaletteImage = bitmap1;
            if (TestImage != null)
            {
                ParseImage();
            }
            await Task.Delay(250);
        }
    }

    public void ExportImageWithRotations(string path)
    {
        var rotations = PaletteRotations;
        var repeat = false;
        var paletteData = PaletteData;
        var paletteOffset = PaletteOffset;
        var paletteLength = PaletteLength;
        var rotationCount = RotationCount;

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

            PaletteLength = paletteLength;
        }

        switch (ImageService.Instance.PaletteType)
        {
            case CdiPaletteType.RGB:
                Palette = ColorHelper.ConvertBytesToRGB(paletteData.Skip(paletteOffset).Take(paletteLength)
                    .ToArray());
                break;
            case CdiPaletteType.Indexed:
                Palette = ColorHelper.ReadPalette(paletteData.Skip(paletteOffset).Take(paletteLength).ToArray());
                break;
            case CdiPaletteType.ClutBanks:
                Palette = ColorHelper.ReadClutBankPalettes(paletteData.Skip(paletteOffset).ToArray(),
                    (byte)paletteLength);
                break;
        }

        var images = new List<System.Drawing.Image>();
        for (int i = 0; i < rotationCount; i++)
        {
            if (i + 1 == rotationCount && repeat) i = 0;
            foreach (var rotation in rotations)
            {
                if (rotation.reverseRotation)
                {
                    ColorHelper.ReverseRotateSubset(Palette, rotation.StartIndex, rotation.EndIndex,
                        rotation.Permutations);
                }
                else
                {
                    ColorHelper.RotateSubset(Palette, rotation.StartIndex, rotation.EndIndex,
                        rotation.Permutations);
                }
            }

            System.Drawing.Bitmap img = new System.Drawing.Bitmap(ImageWidth, ImageHeight);
            if (ImageService.Instance.ImageBytes?.Length > 0)
            {
                switch (ImageService.Instance.VideoType)
                {
                    case CdiVideoType.CLUT7:
                        img = ImageFormatHelper.GenerateClutImage(Palette, ImageService.Instance.ImageBytes, ImageWidth,
                            ImageHeight);
                        break;
                    case CdiVideoType.RL7:
                    default:
                        img = new System.Drawing.Bitmap(ImageFormatHelper.GenerateRle7Image(Palette,
                            ImageService.Instance.ImageBytes, ImageWidth, ImageHeight));
                        break;
                }

                // add img to list
                images.Add(img);
            }
        }
        // set output path to the same folder as the input file
        var outputPath = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, "output");
        Directory.CreateDirectory(outputPath);
        outputPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(path)  + ".gif");
        ImageFormatHelper.CreateGifFromImageList(images, outputPath,50, 0,null, ImageWidth, ImageHeight);
    }
}