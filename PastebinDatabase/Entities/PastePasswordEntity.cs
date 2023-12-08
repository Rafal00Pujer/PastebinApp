using Microsoft.EntityFrameworkCore;
using PastebinDatabase.EntityConfigurations;

namespace PastebinDatabase.Entities;

[EntityTypeConfiguration(typeof(PastePasswordConfiguration))]
public class PastePasswordEntity
{
    public Guid PasteId { get; set; }

    public required string PasswordHash { get; set; }

    public required string PasswordSalt { get; set; }

    public virtual PasteMetaEntity Meta { get; set; } = null!;
}