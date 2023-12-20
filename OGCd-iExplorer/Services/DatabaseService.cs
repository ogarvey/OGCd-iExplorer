using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OGCdiExplorer.Models;

namespace OGCdiExplorer.Services;

public class DatabaseService
{
    private static Lazy<DatabaseService> _instance = new Lazy<DatabaseService>(() => new DatabaseService());
    public static DatabaseService Instance => _instance.Value;
    
    public async Task<List<CdiTitle>> GetCdiTitles()
    {
        await using var context = new CdiContext();
        return await context.CdiTitles.ToListAsync();
    }
    public async Task<bool> CreateCdiTitle(CdiTitle title)
    {
        using (var context = new CdiContext())
        {
            await context.CdiTitles.AddAsync(title);
            await context.SaveChangesAsync();
            return true;
        }
    }

    public async Task<bool> CreateCdiFile(CdiFile file)
    {
        using (var context = new CdiContext())
        {
            await context.CdiFiles.AddAsync(file);
            await context.SaveChangesAsync();
            return true;
        }
    }
}