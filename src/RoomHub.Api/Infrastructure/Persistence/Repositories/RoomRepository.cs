using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public sealed class RoomRepository(RoomHubDbContext db) : IRoomRepository
{
    public Task<Room?> GetByIdWithBookingsAsync(Guid id, CancellationToken cancellationToken) =>
        db.Rooms
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<Room?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken) =>
        db.Rooms
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.Bookings.Any(b => b.Id == bookingId), cancellationToken);

    // Only marks the room as added; the handler saves through IUnitOfWork.
    public void Add(Room room) => db.Rooms.Add(room);
}
