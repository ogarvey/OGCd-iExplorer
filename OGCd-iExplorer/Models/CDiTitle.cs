using System;
using System.Collections.Generic;

namespace OGCdiExplorer.Models;

public class CdiTitle
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ReleaseYear { get; set; }
    public string BasePath { get; set; }
    public string Publisher { get; set; }
    public string DevelopmentCompany { get; set; }
    
    public List<Developer> Developers { get; } = new();
    public List<CdiFile> CdiFiles { get; } = new();
    
}

