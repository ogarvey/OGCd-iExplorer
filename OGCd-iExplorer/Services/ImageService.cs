using System;
using OGLibCDi.Enums;

namespace OGCdiExplorer.Services;

public class ImageService
{
    private static readonly Lazy<ImageService> _instance = new Lazy<ImageService>(() => new ImageService());
    private byte[]? _imageBytes;
    private byte[]? _paletteBytes;
    private CdiVideoType _videoType;
    public static ImageService Instance => _instance.Value;
    
    public event EventHandler ImageBytesChanged;
    public event EventHandler PaletteBytesChanged;
    
    public byte[]? ImageBytes
    {
        get => _imageBytes;
        set
        {
            _imageBytes = value;
            ImageBytesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public byte[]? PaletteBytes
    {
        get => _paletteBytes;
        set
        {
            _paletteBytes = value;
            PaletteBytesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public CdiVideoType VideoType
    {
        get => _videoType;
        set => _videoType = value;
    }
}