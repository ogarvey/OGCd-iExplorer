namespace OGCdiExplorer.Models;

public class CdiFile
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public string FileSize { get; set; }
    public string FileHash { get; set; }
    public string Notes { get; set; }
    
    public int CdiTitleId { get; set; }
    public CdiTitle CdiTitle { get; set; }
}