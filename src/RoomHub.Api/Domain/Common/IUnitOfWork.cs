namespace RoomHub.Api.Domain.Common;

// Implemented by the DbContext; hides EF Core behind a domain contract.
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
