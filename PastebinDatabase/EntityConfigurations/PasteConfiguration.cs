using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PastebinDatabase.Entities;

namespace PastebinDatabase.EntityConfigurations;

internal class PasteConfiguration : IEntityTypeConfiguration<PasteEntity>
{
    public void Configure(EntityTypeBuilder<PasteEntity> builder)
    {
        builder.ToTable("Pastes");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(25)
            .IsRequired(false);

        builder.HasOne(p => p.Meta)
            .WithOne(pm => pm.Paste)
            .HasForeignKey<PasteMetaEntity>()
            .IsRequired();
    }
}