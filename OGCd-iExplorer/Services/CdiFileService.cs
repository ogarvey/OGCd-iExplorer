using System;
using System.IO;
using OGLibCDi.Models;

namespace OGCdiExplorer.Services;

public class CdiFileService
{
    private static readonly Lazy<CdiFileService> _instance = new Lazy<CdiFileService>(() => new CdiFileService());
    public static CdiFileService Instance => _instance.Value;

    public string? CdiFilePath { get; set; }
    public string? CdiFileName { get; set; }
    public string? CdiFileDirectory { get; set; }
    public string? CdiFileExtension { get; set; }

    private CdiFile _cdiFile;

    public CdiFile CdiFile
    {
        get => _cdiFile;
        private set
        {
            _cdiFile = value;
            CdiFileChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler CdiFileChanged;

    public void UpdateCdiFile(CdiFile cdiFile)
    {
        CdiFilePath = cdiFile.FilePath;
        CdiFileName = cdiFile.FileName;
        CdiFileDirectory = Path.GetDirectoryName(CdiFilePath);
        CdiFileExtension = cdiFile.FileExtension;
        CdiFile = cdiFile;
    }
}