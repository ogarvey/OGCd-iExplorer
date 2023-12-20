using System;
using Microsoft.EntityFrameworkCore;

namespace OGCdiExplorer.Models;

public class CdiContext :DbContext
{
    public DbSet<CdiTitle> CdiTitles { get; set; }
    public DbSet<CdiFile> CdiFiles { get; set; }
    public DbSet<CdiSector> CdiSectors { get; set; }
    public DbSet<Developer> Developers { get; set; }
    public string DbPath { get; }
    
    public CdiContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "og-cdi-explorer-new.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}