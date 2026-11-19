using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees) : ICommand<CreateBookingResult>;

public sealed record CreateBookingResult(Guid Id, Guid RoomId, BookingStatus Status);

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CreateBookingCommand, CreateBookingResult>
{
    public async Task<Result<CreateBookingResult>> Handle(
        CreateBookingCommand command, CancellationToken cancellationToken)
    {
        // 1. Load. A missing room is a 404, before any validation.
        var room = await rooms.GetByIdWithBookingsAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        // R9 spans all rooms, so the handler loads the count and the domain decides.
        var bookedBy = command.BookedBy?.Trim() ?? string.Empty;
        var dayUtc = DateRange.ToUtc(command.StartUtc).Date;
        var activeBookingsThatDay = await bookings.CountActiveForPersonOnDayAsync(bookedBy, dayUtc, cancellationToken);

        // 2. The domain applies all the rules (R2-R7, R9, R10).
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var result = room.Book(
            command.BookedBy, command.StartUtc, command.EndUtc, command.Attendees, activeBookingsThatDay, nowUtc);
        if (result.IsFailure)
            return result.Error;

        // 3. Save once.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Return a small result, not the entity.
        var booking = result.Value;
        return new CreateBookingResult(booking.Id, booking.RoomId, booking.Status);
    }
}
