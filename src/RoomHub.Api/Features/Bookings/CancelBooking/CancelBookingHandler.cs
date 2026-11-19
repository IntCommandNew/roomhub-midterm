using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid BookingId) : ICommand<Unit>;

public sealed class CancelBookingHandler(
    IRoomRepository rooms,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CancelBookingCommand, Unit>
{
    public async Task<Result<Unit>> Handle(CancelBookingCommand command, CancellationToken cancellationToken)
    {
        // The booking belongs to the Room aggregate, so we load the room that owns it.
        var room = await rooms.GetByBookingIdAsync(command.BookingId, cancellationToken);
        if (room is null)
            return BookingErrors.NotFound;

        // The domain applies R8 (already cancelled) and R11 (at least 60 minutes before the start).
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var result = room.CancelBooking(command.BookingId, nowUtc);
        if (result.IsFailure)
            return result.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
