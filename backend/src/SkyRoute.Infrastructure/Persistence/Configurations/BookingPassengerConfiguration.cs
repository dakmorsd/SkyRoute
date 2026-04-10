using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence.Configurations;

public sealed class BookingPassengerConfiguration : IEntityTypeConfiguration<BookingPassenger>
{
    public void Configure(EntityTypeBuilder<BookingPassenger> builder)
    {
        builder.ToTable("booking_passengers");
        builder.HasKey(passenger => passenger.Id);

        builder.Property(passenger => passenger.FullName)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(passenger => passenger.Email)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(passenger => passenger.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(passenger => passenger.DocumentNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(passenger => passenger.Booking)
            .WithMany(booking => booking.Passengers)
            .HasForeignKey(passenger => passenger.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}