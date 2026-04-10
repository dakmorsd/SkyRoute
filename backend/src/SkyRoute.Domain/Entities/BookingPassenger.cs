using SkyRoute.Domain.Enums;

namespace SkyRoute.Domain.Entities;

public sealed class BookingPassenger
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
}