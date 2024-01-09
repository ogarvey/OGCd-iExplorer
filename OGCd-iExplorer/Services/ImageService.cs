using System;
using OGLibCDi.Enums;

namespace OGCdiExplorer.Services;

public class ImageService
{
    private static readonly Lazy<ImageService> _instance = new Lazy<ImageService>(() => new ImageService());
    public static ImageService Instance => _instance.Value;
    
    private byte[]? _imageBytes;
    public byte[]? ImageBytes
    {
        get => _imageBytes;
        set => _imageBytes = value;
    }
    
    private byte[]? _paletteBytes;
    public byte[]? PaletteBytes
    {
        get => _paletteBytes;
        set => _paletteBytes = value;
    }
    
    private CdiVideoType _videoType;
    public CdiVideoType VideoType
    {
        get => _videoType;
        set => _videoType = value;
    }
    
    private CdiPaletteType _paletteType;
    public CdiPaletteType PaletteType
    {
        get => _paletteType;
        set => _paletteType = value;
    }
    
    private int _paletteOffset;
    public int PaletteOffset
    {
        get => _paletteOffset;
        set => _paletteOffset = value;
    }
    
    private int _paletteLength;
    public int PaletteLength
    {
        get => _paletteLength;
        set => _paletteLength = value;
    }
    
    private int _imageOffset;
    public int ImageOffset
    {
        get => _imageOffset;
        set => _imageOffset = value;
    }
    
    private int _imageLength;
    public int ImageLength
    {
        get => _imageLength;
        set => _imageLength = value;
    }
    
    private int _imageWidth;
    public int ImageWidth
    {
        get => _imageWidth;
        set => _imageWidth = value;
    }
    
    private int _imageHeight;
    public int ImageHeight
    {
        get => _imageHeight;
        set => _imageHeight = value;
    }

    private uint _initialY = 128;
    public uint InitialY
    {
        get => _initialY;
        set => _initialY = value;
    }
    
    private uint _initialU = 128;
    public uint InitialU
    {
        get => _initialU;
        set => _initialU = value;
    }
    
    private uint _initialV = 128;
    public uint InitialV
    {
        get => _initialV;
        set => _initialV = value;
    }
}