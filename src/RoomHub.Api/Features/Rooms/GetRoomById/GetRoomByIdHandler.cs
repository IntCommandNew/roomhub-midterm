using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Features.Rooms.GetRoomById;

public sealed record GetRoomByIdQuery(Guid Id) : IQuery<RoomDetails>;

public sealed record RoomDetails(Guid Id, string Name, int Capacity);

// Read side: queries the DbContext directly, without tracking, and projects to a DTO.
public sealed class GetRoomByIdHandler(RoomHubDbContext db) : IQueryHandler<GetRoomByIdQuery, RoomDetails>
{
    public async Task<Result<RoomDetails>> Handle(GetRoomByIdQuery query, CancellationToken cancellationToken)
    {
        var room = await db.Rooms
            .AsNoTracking()
            .Where(r => r.Id == query.Id)
            .Select(r => new RoomDetails(r.Id, r.Name, r.Capacity))
            .FirstOrDefaultAsync(cancellationToken);

        if (room is null)
            return RoomErrors.NotFound;

        return room;
    }
}
