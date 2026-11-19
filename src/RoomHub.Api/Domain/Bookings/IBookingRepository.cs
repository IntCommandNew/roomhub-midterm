namespace RoomHub.Api.Domain.Bookings;

// Query across ALL rooms, needed by R9 (a rule that spans several Room aggregates).
public interface IBookingRepository
{
    // Number of ACTIVE bookings of bookedBy whose start falls on the UTC day dayUtc (00:00 inclusive, next 00:00 exclusive).
    Task<int> CountActiveForPersonOnDayAsync(string bookedBy, DateTime dayUtc, CancellationToken cancellationToken);
}
