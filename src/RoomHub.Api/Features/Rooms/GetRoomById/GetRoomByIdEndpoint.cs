using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Rooms.GetRoomById;

public static class GetRoomByIdEndpoint
{
    public sealed record Response(Guid Id, string Name, int Capacity);

    public static void Map(RouteGroupBuilder group) =>
        group.MapGet("/{id:guid}", Handle).WithName("GetRoomById");

    private static async Task<Results<Ok<Response>, ProblemHttpResult>> Handle(
        Guid id,
        IQueryHandler<GetRoomByIdQuery, RoomDetails> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new GetRoomByIdQuery(id), cancellationToken);

        if (result.IsFailure)
            return result.Error.ToProblem();

        var room = result.Value;
        return TypedResults.Ok(new Response(room.Id, room.Name, room.Capacity));
    }
}
