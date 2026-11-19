using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

// Value object: the interval [StartUtc, EndUtc). Value equality, no identity.
public sealed record DateRange
{
    private DateRange() { } // for EF Core

    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }

    public TimeSpan Duration => EndUtc - StartUtc;

    public static Result<DateRange> Create(DateTime start, DateTime end)
    {
        var startUtc = ToUtc(start);
        var endUtc = ToUtc(end);

        if (endUtc <= startUtc)
            return BookingErrors.InvalidRange; // R3

        return new DateRange { StartUtc = startUtc, EndUtc = endUtc };
    }

    // Half-open: 10-12 and 12-14 do NOT overlap; 10-12 and 11-13 do. (R7)
    public bool Overlaps(DateRange other) => StartUtc < other.EndUtc && other.StartUtc < EndUtc;

    public static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc) // Unspecified: treat as UTC
    };
}
