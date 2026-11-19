using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Rooms;

// Aggregate root: owns its bookings and enforces the rules that span them.
public sealed class Room
{
    private readonly List<Booking> _bookings = [];

    private Room() { } // for EF Core

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }

    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

    // R1: non-empty name, strictly positive capacity.
    public static Result<Room> Create(string? name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoomErrors.NameRequired;
        if (capacity <= 0)
            return RoomErrors.InvalidCapacity;

        return new Room
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Capacity = capacity
        };
    }

    // activeBookingsOfPersonThatDay: how many ACTIVE bookings bookedBy already has on the UTC day
    // of startUtc, across ALL rooms (loaded by the handler, because it spans aggregates).
    public Result<Booking> Book(
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        int activeBookingsOfPersonThatDay,
        DateTime nowUtc)
    {
        // R2
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.BookedByRequired;

        // R3
        var rangeResult = DateRange.Create(startUtc, endUtc);
        if (rangeResult.IsFailure)
            return rangeResult.Error;
        var range = rangeResult.Value;

        // R4
        if (attendees <= 0)
            return BookingErrors.InvalidAttendees;
        if (attendees > Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        // R5: "now" is passed in (from TimeProvider), never DateTime.UtcNow here.
        if (range.StartUtc < nowUtc)
            return BookingErrors.StartInPast;

        // R6: exactly 4 hours is allowed.
        if (range.Duration > Booking.MaxDuration)
            return BookingErrors.TooLong;

        // R7: only ACTIVE bookings block; touching intervals do not overlap (half-open).
        if (_bookings.Any(b => b.IsActive && b.Range.Overlaps(range)))
            return BookingErrors.Overlapping;

        // R9: at most 2 active bookings per person per UTC day, across all rooms.
        if (activeBookingsOfPersonThatDay >= Booking.MaxActiveBookingsPerPersonPerDay)
            return BookingErrors.DailyLimitReached;

        var booking = Booking.Create(Id, bookedBy.Trim(), range, attendees);
        _bookings.Add(booking);
        return booking;
    }

    // R8
    public Result<Unit> CancelBooking(Guid bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
        if (booking is null)
            return BookingErrors.NotFound;

        return booking.Cancel();
    }
}
