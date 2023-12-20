using System.Collections.Generic;

namespace OGCdiExplorer.Models;

public class Developer
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public List<CdiTitle> CdiTitles { get; } = new();
}