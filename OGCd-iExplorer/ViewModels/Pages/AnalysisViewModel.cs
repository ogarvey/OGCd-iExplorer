using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using OGCdiExplorer.Services;
using OGLibCDi.Enums;
using OGLibCDi.Helpers;
using OGLibCDi.Models;
using ReactiveUI;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace OGCdiExplorer.ViewModels.Pages;

public class AnalysisViewModel : PageViewModel
{
    public override bool CanNavigateNext { get; protected set; }
    public override bool CanNavigatePrevious { get; protected set; }
    
    public AnalysisViewModel()
    {
        SelectedCdiFile = CdiFileService.Instance.CdiFile;
        FilteredItems = new ObservableCollection<CdiSector>(SelectedCdiFile.Sectors);
        Sectors = new ObservableCollection<CdiSector>(SelectedCdiFile.Sectors);
        CdiFileService.Instance.CdiFileChanged += (sender, args) =>
        {
            SelectedCdiFile = CdiFileService.Instance.CdiFile;
            FilteredItems = new ObservableCollection<CdiSector>(SelectedCdiFile.Sectors);
            Sectors = new ObservableCollection<CdiSector>(SelectedCdiFile.Sectors);
        };
    }
    
    private Bitmap _previewImage;

    public Bitmap PreviewImage
    {
        get => _previewImage;
        set => this.RaiseAndSetIfChanged(ref _previewImage, value);
    }
    
    private Bitmap _previewPalette;
    public Bitmap PreviewPalette
    {
        get => _previewPalette;
        set => this.RaiseAndSetIfChanged(ref _previewPalette, value);
    }
    
    private int _imageOffset = 0;
    public int ImageOffset
    {
        get => _imageOffset;
        set => this.RaiseAndSetIfChanged(ref _imageOffset, value);
    }
    
    private int _imageLength = 92160;
    public int ImageLength
    {
        get => _imageLength;
        set => this.RaiseAndSetIfChanged(ref _imageLength, value);
    }
    
    private int _imageWidth = 384;
    public int ImageWidth
    {
        get => _imageWidth;
        set => this.RaiseAndSetIfChanged(ref _imageWidth, value);
    }
    
    private int _imageHeight = 240;
    public int ImageHeight
    {
        get => _imageHeight;
        set => this.RaiseAndSetIfChanged(ref _imageHeight, value);
    }
    
    private uint _initialY = 128;
    public uint InitialY
    {
        get => _initialY;
        set => this.RaiseAndSetIfChanged(ref _initialY, value);
    }
    
    private uint _initialU = 128;
    public uint InitialU
    {
        get => _initialU;
        set => this.RaiseAndSetIfChanged(ref _initialU, value);
    }
    
    private uint _initialV = 128;
    public uint InitialV
    {
        get => _initialV;
        set => this.RaiseAndSetIfChanged(ref _initialV, value);
    }
    
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
    
    private ObservableCollection<CdiSector>? _sectors;

    public ObservableCollection<CdiSector>? Sectors { 
        get => _sectors;
        set => this.RaiseAndSetIfChanged(ref _sectors, value);
    } 
    
    private ObservableCollection<CdiSector>? _filteredItems;

    public ObservableCollection<CdiSector>? FilteredItems
    {
        get => _filteredItems;
        set => this.RaiseAndSetIfChanged(ref _filteredItems, value);
    }
    
    private bool _filterAudio = false;
    public bool FilterAudio
    {
        get => _filterAudio;
        set => this.RaiseAndSetIfChanged(ref _filterAudio, value);
    }

    private bool _filterVideo = false;
    public bool FilterVideo
    {
        get => _filterVideo;
        set => this.RaiseAndSetIfChanged(ref _filterVideo, value);
    }

    private bool _filterData = false;
    private CdiVideoType _videoType;
    public CdiVideoType VideoType
    {
        get => _videoType;
        set => this.RaiseAndSetIfChanged(ref _videoType, value);
    }
    public bool FilterData
    {
        get => _filterData;
        set => this.RaiseAndSetIfChanged(ref _filterData, value);
    }

    private byte[]? _imageBytes;
    public byte[]? ImageBytes
    {
        get => _imageBytes;
        set => this.RaiseAndSetIfChanged(ref _imageBytes, value);
    }
    
    private byte[]? _audioBytes;
    public byte[]? AudioBytes
    {
        get => _audioBytes;
        set => this.RaiseAndSetIfChanged(ref _audioBytes, value);
    }
    
    private byte[]? _paletteBytes;
    public byte[]? PaletteBytes
    {
        get => _paletteBytes;
        set => this.RaiseAndSetIfChanged(ref _paletteBytes, value);
    }
    
    private int _filterChannel = -1;
    public int FilterChannel
    {
        get => _filterChannel;
        set => this.RaiseAndSetIfChanged(ref _filterChannel, value);
    }
    
    private int _filterVideoType = -1;
    public int FilterVideoType
    {
        get => _filterVideoType;
        set => this.RaiseAndSetIfChanged(ref _filterVideoType, value);
    }

    private bool _isMono = true;
    public bool IsMono
    {
        get => _isMono;
        set => this.RaiseAndSetIfChanged(ref _isMono, value);
    }
    
    private int _frequency = 18900;
    public int Frequency
    {
        get => _frequency;
        set => this.RaiseAndSetIfChanged(ref _frequency, value);
    }
    
    private int _bitsPerSample = 4;
    public int BitsPerSample
    {
        get => _bitsPerSample;
        set => this.RaiseAndSetIfChanged(ref _bitsPerSample, value);
    }
    
    private int _sectorCount = 0;
    public int SectorCount
    {
        get => _sectorCount;
        set => this.RaiseAndSetIfChanged(ref _sectorCount, value);
    }

    public void ApplySectorTypeFilters()
    {
        var filtered = _sectors?.Where(item =>
            (!FilterAudio || item.SectorTypeString == "Audio") ||
            (!FilterVideo || item.SectorTypeString == "Video") ||
            (!FilterData || item.SectorTypeString == "Data")).OrderBy(s => s.SectorIndex).ToList();
        
        filtered = filtered?.Where(item => (FilterChannel != -1 && item.Channel == FilterChannel) || (FilterChannel == -1)).ToList();
        filtered = filtered?.Where(item => (FilterVideoType != -1 && item.Coding.Coding == FilterVideoType) || (FilterVideoType == -1)).ToList();
        
        if (filtered != null) FilteredItems = new ObservableCollection<CdiSector>(filtered);
    }

    private MemoryStream _memoryStream = new MemoryStream();
    public MemoryStream AMemoryStream
    {
        get => _memoryStream;
        set => this.RaiseAndSetIfChanged(ref _memoryStream, value);
    }
    
    private string _toFormat = "CLUT7";
    public string ToFormat { get => _toFormat; set => this.RaiseAndSetIfChanged(ref _toFormat, value); }

    public void PopulatePalette()
    {
        PaletteBytes = ImageService.Instance.PaletteBytes;
        if (PaletteBytes?.Length == 0) return;
        PaletteLength = ImageService.Instance.PaletteLength;
        PaletteOffset = ImageService.Instance.PaletteOffset;
        var palette = new List<Color>();
        
        switch (ImageService.Instance.PaletteType)
        {
            case CdiPaletteType.RGB:
                palette = ColorHelper.ConvertBytesToRGB(PaletteBytes.Skip(PaletteOffset).Take(PaletteLength)
                    .ToArray());
                break;
            case CdiPaletteType.Indexed:
                palette = ColorHelper.ReadPalette(PaletteBytes.Skip(PaletteOffset).Take(PaletteLength).ToArray());
                break;
            case CdiPaletteType.ClutBanks:
                palette = ColorHelper.ReadClutBankPalettes(PaletteBytes.Skip(PaletteOffset).ToArray(),
                    (byte)PaletteLength);
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
        PreviewPalette = bitmap1;
    }
    public void PopulateImage()
    {
        VideoType = ImageService.Instance.VideoType;
        ImageBytes = ImageService.Instance.ImageBytes;
        PaletteBytes = ImageService.Instance.PaletteBytes;
        PaletteLength = ImageService.Instance.PaletteLength;
        PaletteOffset = ImageService.Instance.PaletteOffset;
        ImageOffset = ImageService.Instance.ImageOffset;
        ImageLength = ImageService.Instance.ImageLength;
        ImageWidth = ImageService.Instance.ImageWidth;
        ImageHeight = ImageService.Instance.ImageHeight;
        InitialY = ImageService.Instance.InitialY;
        InitialU = ImageService.Instance.InitialU;
        InitialV = ImageService.Instance.InitialV;
        
        if (ImageBytes?.Length == 0) return;

        var palette = new List<Color>();

        if (VideoType != CdiVideoType.DYUV)
        {
            if (PaletteBytes?.Length == 0) return;
            switch (ImageService.Instance.PaletteType)
            {
                case CdiPaletteType.RGB:
                    palette = ColorHelper.ConvertBytesToRGB(PaletteBytes.Skip(PaletteOffset).Take(PaletteLength)
                        .ToArray());
                    break;
                case CdiPaletteType.Indexed:
                    palette = ColorHelper.ReadPalette(PaletteBytes.Skip(PaletteOffset).Take(PaletteLength).ToArray());
                    break;
                case CdiPaletteType.ClutBanks:
                    palette = ColorHelper.ReadClutBankPalettes(PaletteBytes.Skip(PaletteOffset).ToArray(),
                        (byte)PaletteLength);
                    break;
            }
        }

        System.Drawing.Bitmap img = new System.Drawing.Bitmap(ImageWidth, ImageHeight);
        switch (VideoType)
        {
            case CdiVideoType.DYUV:
                var y = ImageService.Instance.InitialY;
                var u = ImageService.Instance.InitialU;
                var v = ImageService.Instance.InitialV;
                img = ImageFormatHelper.DecodeDYUVImage(ImageBytes.Skip(ImageOffset).Take(ImageLength).ToArray(),
                    ImageWidth, ImageHeight,y,u,v);
                break;
            case CdiVideoType.RL7:
                var bytes = ImageFormatHelper.DecodeRle(ImageBytes.Skip(ImageOffset).Take(ImageLength).ToArray(),
                    ImageWidth);
                var image = Utilities.CreateImage(bytes, palette, ImageWidth, ImageHeight, false);
                img = new System.Drawing.Bitmap(image);
                break;
            case CdiVideoType.CLUT7:
            case CdiVideoType.CLUT8:
                img = ImageFormatHelper.GenerateClutImage(palette,
                    ImageBytes.Skip(ImageOffset).Take(ImageLength).ToArray(), ImageWidth, ImageHeight);
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
        PreviewImage = bitmap1;
    }

    public void PopulateAudio()
    {
        AMemoryStream = new MemoryStream();
        List<short> left = new List<short>();
        List<short> right = new List<short>();

        for (int i = 0; i < AudioBytes?.Length; i += 2304)
        {
            byte[] chunk = new byte[2304];
            Array.Copy(AudioBytes, i, chunk, 0, 2304);
            try
            {
                AudioHelper.DecodeAudioSector(chunk, left, right, BitsPerSample == 8, !IsMono);

            }
            catch
            {
                continue;
            }
        }

        AudioHelper.WAVHeader header = new AudioHelper.WAVHeader
        {
            ChannelNumber = (ushort)(IsMono ? 1 : 2), // Mono
            Frequency = (uint)Frequency, // 18.9 kHz
        };
        
        AudioHelper.WriteWAV(AMemoryStream, header, left, right);
    }
}