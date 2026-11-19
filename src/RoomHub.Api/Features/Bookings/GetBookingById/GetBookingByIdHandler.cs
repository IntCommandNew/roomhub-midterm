using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Features.Bookings.GetBookingById;

public sealed record GetBookingByIdQuery(Guid Id) : IQuery<BookingDetails>;

public sealed record BookingDetails(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    BookingStatus Status);

// Read side: queries the DbContext directly, without tracking, and projects to a DTO.
public sealed class GetBookingByIdHandler(RoomHubDbContext db) : IQueryHandler<GetBookingByIdQuery, BookingDetails>
{
    public async Task<Result<BookingDetails>> Handle(GetBookingByIdQuery query, CancellationToken cancellationToken)
    {
        var booking = await db.Bookings
            .AsNoTracking()
            .Where(b => b.Id == query.Id)
            .Select(b => new BookingDetails(
                b.Id, b.RoomId, b.BookedBy, b.Range.StartUtc, b.Range.EndUtc, b.Attendees, b.Status))
            .FirstOrDefaultAsync(cancellationToken);

        if (booking is null)
            return BookingErrors.NotFound;

        return booking;
    }
}
