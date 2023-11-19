using System;
using OGCdiExplorer.Services;
using OGLibCDi.Models;
using ReactiveUI;

namespace OGCdiExplorer.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public ViewModelBase()
    {
        CdiFileService.Instance.CdiFileChanged += UpdateCdiFile;
        SelectedCdiFile = null;
    }

    private void UpdateCdiFile(object? sender, EventArgs eventArgs)
    {
        CdiFilePath = CdiFileService.Instance.CdiFilePath ?? "No File Selected";
        SelectedCdiFile = CdiFileService.Instance.CdiFile;
        IsCdiFileLoaded = true;
    }
    public CdiFile? SelectedCdiFile
    {
        get => _cdiFile;
        set => this.RaiseAndSetIfChanged(ref _cdiFile, value);
    }

    private CdiFile? _cdiFile;

    public string CdiFilePath
    {
        get => _cdiFilePath ?? "No file selected";
        set => this.RaiseAndSetIfChanged(ref _cdiFilePath, value);
    }

    private string? _cdiFilePath;

    public bool IsCdiFileLoaded
    {
        get => _isCdiFileLoaded;
        set => this.RaiseAndSetIfChanged(ref _isCdiFileLoaded, value);
    }

    private bool _isCdiFileLoaded;
}