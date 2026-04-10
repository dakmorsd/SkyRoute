using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");
        builder.HasKey(booking => booking.Id);

        builder.Property(booking => booking.ReferenceCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(booking => booking.ProviderCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(booking => booking.ProviderDisplayName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(booking => booking.FlightNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(booking => booking.OriginAirportCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(booking => booking.DestinationAirportCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(booking => booking.OriginCountryCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(booking => booking.DestinationCountryCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(booking => booking.CabinClass)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(booking => booking.RouteType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(booking => booking.PerPassengerPrice)
            .HasPrecision(10, 2);

        builder.Property(booking => booking.TotalPrice)
            .HasPrecision(10, 2);

        builder.HasIndex(booking => booking.ReferenceCode)
            .IsUnique();

        builder.HasOne(booking => booking.User)
            .WithMany(user => user.Bookings)
            .HasForeignKey(booking => booking.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}