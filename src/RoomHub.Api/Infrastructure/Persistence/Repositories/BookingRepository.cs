using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public sealed class BookingRepository(RoomHubDbContext db) : IBookingRepository
{
    public Task<int> CountActiveForPersonOnDayAsync(string bookedBy, DateTime dayUtc, CancellationToken cancellationToken)
    {
        var dayStart = DateTime.SpecifyKind(dayUtc.Date, DateTimeKind.Utc);
        var nextDayStart = dayStart.AddDays(1);

        return db.Bookings.CountAsync(b =>
                b.Status == BookingStatus.Active &&
                b.BookedBy == bookedBy &&
                b.Range.StartUtc >= dayStart &&
                b.Range.StartUtc < nextDayStart,
            cancellationToken);
    }
}
