namespace OGCdiExplorer.Models;

public class CdiSector
{
    public int Id { get; set; }
    public int SectorIndex { get; set; }
    public int SectorType { get; set; }
    public int FileNumber { get; set; }
    public int Channel { get; set; }
    
    public int CdiFileId { get; set; }
    public CdiFile CdiFile { get; set; }
}

public enum CdiSectorType
{
    Empty = 0,
    Video = 2,
    Audio = 4,
    Data = 8
}