using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class BookingErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Booking.NotFound", "The booking does not exist.");

    public static readonly Error BookedByRequired =
        Error.Validation("Booking.BookedByRequired", "BookedBy is required.");

    public static readonly Error InvalidRange =
        Error.Validation("Booking.InvalidRange", "The end must be strictly after the start.");

    public static readonly Error InvalidAttendees =
        Error.Validation("Booking.InvalidAttendees", "Attendees must be strictly positive.");

    public static readonly Error AttendeesExceedCapacity =
        Error.Validation("Booking.AttendeesExceedCapacity", "Attendees cannot exceed the room capacity.");

    public static readonly Error StartInPast =
        Error.Validation("Booking.StartInPast", "A booking cannot start in the past.");

    public static readonly Error TooLong =
        Error.Validation("Booking.TooLong", "A booking cannot last more than 4 hours.");

    public static readonly Error Overlapping =
        Error.Conflict("Booking.Overlapping", "The room is already booked in this interval.");

    public static readonly Error AlreadyCancelled =
        Error.Conflict("Booking.AlreadyCancelled", "The booking is already cancelled.");

    public static readonly Error DailyLimitReached =
        Error.Conflict("Booking.DailyLimitReached",
            "A participant cannot have more than 2 active bookings on the same day.");
}
