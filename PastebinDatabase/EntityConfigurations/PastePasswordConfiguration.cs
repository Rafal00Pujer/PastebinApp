using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PastebinDatabase.Entities;

namespace PastebinDatabase.EntityConfigurations;

internal class PastePasswordConfiguration : IEntityTypeConfiguration<PastePasswordEntity>
{
    public void Configure(EntityTypeBuilder<PastePasswordEntity> builder)
    {
        builder.ToTable("Pastes_Passwords");

        builder.HasKey(pp => pp.PasteId);

        builder.HasOne(pp => pp.Meta)
            .WithOne(pm => pm.Password)
            .HasForeignKey<PasteEntity>();
    }
}