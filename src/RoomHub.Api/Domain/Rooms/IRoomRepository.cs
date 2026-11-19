namespace RoomHub.Api.Domain.Rooms;

// Write-side contract for the Room aggregate. No EF Core types here; never saves.
public interface IRoomRepository
{
    // Loads the room with its bookings (needed for the overlap rule R7).
    Task<Room?> GetByIdWithBookingsAsync(Guid id, CancellationToken cancellationToken);

    // Loads the room that owns the given booking, with its bookings (needed to cancel, R8).
    Task<Room?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken);

    void Add(Room room);
}
