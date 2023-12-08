using Microsoft.EntityFrameworkCore;
using PastebinDatabase.EntityConfigurations;

namespace PastebinDatabase.Entities;

[EntityTypeConfiguration(typeof(PasteConfiguration))]
public class PasteEntity
{
    public Guid Id { get; set; }

    public required string Content { get; set; }

    public string? Name { get; set; }

    public virtual PasteMetaEntity Meta { get; set; } = null;

    public virtual PastePasswordEntity? Password { get; set; }
}