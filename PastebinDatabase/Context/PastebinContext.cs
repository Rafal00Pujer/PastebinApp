using Microsoft.EntityFrameworkCore;
using PastebinDatabase.Entities;

namespace PastebinDatabase.Context;

public class PastebinContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<PasteEntity> Pastes { get; set; }

    public DbSet<PasteMetaEntity> PastesMetas { get; set; }

    public DbSet<PastePasswordEntity> PastesPasswords { get; set; }
}