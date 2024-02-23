using System.Collections.Generic;
using OGLibCDi.Helpers;
using OGLibCDi.Models.Bolt;
using ReactiveUI;

namespace OGCdiExplorer.ViewModels.Windows;

public class BoltFileParserViewModel : ReactiveObject
{
    private byte[]? _fileBytes;
    public byte[]? FileBytes
    {
        get => _fileBytes;
        set {
            if (value.Length > 0)
            {
                BoltOffsets = BoltFileHelper.GetBoltOffsetData(value);
            }
            this.RaiseAndSetIfChanged(ref _fileBytes, value);
        }
    }

    private bool _showFileReceiver = true;
    public bool ShowFileReceiver
    {
        get => _showFileReceiver;
        set => this.RaiseAndSetIfChanged(ref _showFileReceiver, value);
    }
    
    private List<BoltOffset>? _boltOffsets;
    public List<BoltOffset>? BoltOffsets
    {
        get => _boltOffsets;
        set => this.RaiseAndSetIfChanged(ref _boltOffsets, value);
    }

    public void OpenCommand()
    {
        
    }
    
    public void ExitCommand()
    {
        
    }
}