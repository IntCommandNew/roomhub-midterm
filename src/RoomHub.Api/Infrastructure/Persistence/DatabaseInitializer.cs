namespace RoomHub.Api.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    // Creates the SQLite schema on first start. Existing data is kept, so it survives a restart.
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<RoomHubDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
