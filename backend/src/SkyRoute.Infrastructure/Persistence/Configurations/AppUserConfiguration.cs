using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.FullName)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();
    }
}