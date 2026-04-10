using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence.Configurations;

public sealed class ProviderDefinitionConfiguration : IEntityTypeConfiguration<ProviderDefinition>
{
    public void Configure(EntityTypeBuilder<ProviderDefinition> builder)
    {
        builder.ToTable("providers");
        builder.HasKey(provider => provider.Code);

        builder.Property(provider => provider.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(provider => provider.DisplayName)
            .HasMaxLength(100)
            .IsRequired();
    }
}