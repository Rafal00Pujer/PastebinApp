using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PastebinDatabase.Entities;

namespace PastebinDatabase.EntityConfigurations;

internal class PasteMetaConfiguration : IEntityTypeConfiguration<PasteMetaEntity>
{
    public void Configure(EntityTypeBuilder<PasteMetaEntity> builder)
    {
        builder.ToTable("Pastes_Metas");

        builder.HasKey(m => m.PasteId);

        builder.HasOne(pm => pm.Password)
            .WithOne(pp => pp.Meta)
            .HasForeignKey<PastePasswordEntity>()
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}