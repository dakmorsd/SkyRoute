using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence.Configurations;

public sealed class AirportConfiguration : IEntityTypeConfiguration<Airport>
{
    public void Configure(EntityTypeBuilder<Airport> builder)
    {
        builder.ToTable("airports");
        builder.HasKey(airport => airport.Code);

        builder.Property(airport => airport.Code)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(airport => airport.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(airport => airport.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(airport => airport.CountryCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(airport => airport.CountryName)
            .HasMaxLength(100)
            .IsRequired();
    }
}