using ReactiveUI;

namespace OGCdiExplorer.ViewModels.Pages;

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
    
    private byte[] _paletteData = new byte[384];
    public byte[] PaletteData
    {
        get => _paletteData;
        set => this.RaiseAndSetIfChanged(ref _paletteData, value);
    }
    
    
}