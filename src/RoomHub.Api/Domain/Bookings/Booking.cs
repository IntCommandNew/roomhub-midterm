using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

// Child entity of the Room aggregate: created only through Room.Book.
public sealed class Booking
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4); // R6
    public const int MaxActiveBookingsPerPersonPerDay = 2;               // R9
    public static readonly TimeSpan MinCancellationNotice = TimeSpan.FromMinutes(60); // R11

    private Booking() { } // for EF Core

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateRange Range { get; private set; } = null!;
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public bool IsActive => Status == BookingStatus.Active;

    internal static Booking Create(Guid roomId, string bookedBy, DateRange range, int attendees) => new()
    {
        Id = Guid.NewGuid(),
        RoomId = roomId,
        BookedBy = bookedBy,
        Range = range,
        Attendees = attendees,
        Status = BookingStatus.Active
    };

    // R8: Active -> Cancelled; cancelling twice is a conflict.
    // R11: only if the booking starts at least 60 minutes from now (exactly 60 is allowed).
    internal Result<Unit> Cancel(DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        if (Range.StartUtc < nowUtc + MinCancellationNotice)
            return BookingErrors.TooLateToCancel;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
