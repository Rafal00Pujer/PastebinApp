using Microsoft.EntityFrameworkCore;
using PastebinDatabase.EntityConfigurations;

namespace PastebinDatabase.Entities;

[EntityTypeConfiguration(typeof(PastePasswordConfiguration))]
public class PastePasswordEntity
{
    public required Guid PasteId { get; set; }

    public required string PasswordHash { get; set; }

    public required string PasswordSalt { get; set; }

    public virtual PasteEntity Paste { get; set; } = null!;

    public virtual PasteMetaEntity Meta { get; set; } = null!;
}