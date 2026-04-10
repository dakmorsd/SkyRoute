using Microsoft.EntityFrameworkCore;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence;

public sealed class SkyRouteDbContext(DbContextOptions<SkyRouteDbContext> options) : DbContext(options)
{
    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<ProviderDefinition> Providers => Set<ProviderDefinition>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingPassenger> BookingPassengers => Set<BookingPassenger>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SkyRouteDbContext).Assembly);
    }
}